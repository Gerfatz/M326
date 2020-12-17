using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Battleships.Commands;
using BusinessLayer;
using BusinessLayer.DocumentGeneration;
using BusinessLayer.Storage;
using Microsoft.Win32;

namespace Battleships.ViewModels
{
    public class PlayingFieldViewModel : INotifyPropertyChanged
    {
        private readonly Fieldsaver _fieldsaver;
        private Field _field;
        private FieldSavingViewModel _selectedField;
        private ObservableCollection<FieldSavingViewModel> _fields;
        private int _sideLength = 7;
        private bool _showResult = false;
        private bool _editMode = true;

        // Properties

        /// <summary>
        /// Represents the switch between deletion and addition mode when editing the playing field
        /// </summary>
        public bool? DeleteState { get; set; } = false;

        /// <summary>
        /// The currently selected <see cref="Field"/>. This is the real field that contains the boats
        /// </summary>
        public Field Field {
            get => _field;
            private set
            {
                _field = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The positions selected by the user
        /// </summary>
        public List<Position> SelectedPositions { get; set; }

        /// <summary>
        /// List of all fields with their names and GUID
        /// </summary>
        public ObservableCollection<FieldSavingViewModel> Fields 
        {
            get => _fields;
            private set
            {
                _fields = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The selected Field in context of saving and loading. Only contains the name and GUID
        /// </summary>
        public FieldSavingViewModel SelectedField
        {
            get => _selectedField;
            set
            {
                if (value != null)
                {
                    try
                    {
                        Field = _fieldsaver.GetField(value.Id);
                        value.PropertyChanged += UpdateFieldName;
                    }
                    catch (ApplicationException ex)
                    {
                        ShowErrorMessage(ex.Message, "Error while loading the field");
                        return;
                    }
                }
                else
                {
                    Field = null;
                }

                if (_selectedField != null)
                {
                    _selectedField.PropertyChanged -= UpdateFieldName;
                }

                _selectedField = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The side length of the field
        /// </summary>
        public int SideLength
        {
            get => _sideLength;
            set
            {
                if(value >= 5 && value <= 20)
                {
                    _sideLength = value;
                    Field = new Field(_sideLength);
                    OnPropertyChanged();
                }
                else
                {
                    MessageBox.Show("Minimum Size is 5. Maximum Size is 20");
                }
            }
        }

        /// <summary>
        /// Switch for showing the results in playing mode
        /// </summary>
        public bool ShowResult
        {
            get => _showResult;
            private set
            {
                _showResult = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Switches between edit and playing mode
        /// </summary>
        public bool EditMode
        {
            get => _editMode;
            private set
            {
                _editMode = value;
                SelectedPositions.Clear();
                OnPropertyChanged();
                OnPropertyChanged(nameof(GameMode));
            }
        }

        /// <summary>
        /// Is the game currently in playing mode. Used for activating UI Elements
        /// </summary>
        public bool GameMode => !_editMode;

        //Commands

        /// <summary>
        /// Command to save the current <see cref="Field"/>
        /// </summary>
        public ActionCommand SaveCommand { get; set; }

        /// <summary>
        /// Command to create a new playing field, and setting it as the currently selected
        /// </summary>
        public ActionCommand NewFieldCommand { get; set; }

        /// <summary>
        /// Toggles the <see cref="ShowResult"/> property
        /// </summary>
        public ActionCommand ShowResultCommand { get; set; }

        /// <summary>
        /// Toggles between edit and playing mode
        /// </summary>
        public ActionCommand ToggleEditCommand { get; set; }

        /// <summary>
        /// Opens a file dialog and exports the currently selected <see cref="Field"/> as a Pdf to the selected location
        /// </summary>
        public ActionCommand ExportCommand { get; set; }

        /// <summary>
        /// Command to delete the currently selected <see cref="Field"/>
        /// </summary>
        public ActionCommand DeleteCommand { get; set; }

        /// <summary>
        /// Command to overwrite all current boats in <see cref="Field"/>, and replaces the with randomly generated ones.
        /// After that it switches to game mode.
        /// </summary>
        public ActionCommand GenerateCommand { get; set; }

        /// <summary>
        /// Event that is triggered when a public property of this object changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates a new instance, and loads a list of all saved playing fields
        /// </summary>
        public PlayingFieldViewModel()
        {
            SelectedPositions = new List<Position>();

            SaveCommand = new ActionCommand(Save, IsFieldNotNull);
            NewFieldCommand = new ActionCommand(NewField);
            ShowResultCommand = new ActionCommand(ShowRes);
            ToggleEditCommand = new ActionCommand(ToggleEdit);
            ExportCommand = new ActionCommand(Export, IsFieldNotNull);
            DeleteCommand = new ActionCommand(Delete, IsFieldNotNull);
            GenerateCommand = new ActionCommand(Generate, IsFieldNotNull);

            _fieldsaver = new Fieldsaver();

            try
            {
                Fields = ConvertSavingDictToViewModel(_fieldsaver.GetAllFields());
            }
            catch (ApplicationException ex)
            {
                ShowErrorMessage(ex.Message, "Error while loading fields");
            }
        }

        /// <summary>
        /// Saves the current field
        /// </summary>
        public void Save()
        {
            try
            {
                _fieldsaver.Save(_field, _selectedField.Id);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Error while saving", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Creates a new field
        /// </summary>
        public void NewField()
        {
            Field field = new Field(_sideLength);
            field.Name = "New Field";
            Guid newFieldGuid;

            try
            {
                newFieldGuid = _fieldsaver.Create(field);
            }
            catch(ApplicationException ex)
            {
                ShowErrorMessage(ex.Message, "Error while creating new field");
                return;
            }

            FieldSavingViewModel newModel = new FieldSavingViewModel
            {
                Id = newFieldGuid,
                Name = field.Name
            };

            Fields.Add(newModel);

            SelectedField = newModel;
            Field = field;
        }

        /// <summary>
        /// Opens a file dialog and exports the current field as a .pdf file
        /// </summary>
        public void Export()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = Field.Name + ".pdf";
            dialog.AddExtension = true;
            dialog.Filter = "Pdf Document|*.pdf";

            if(dialog.ShowDialog() == true)
            {
                try
                {
                    PdfGenerator.Generate(dialog.FileName, Field);
                    MessageBox.Show("Export successful");
                }
                catch(IOException)
                {
                    MessageBox.Show("Export failed. Make sure the seleced File is not in use by any other programm", "Export Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        /// <summary>
        /// Asks the user for confirmation and deletes the current field
        /// </summary>
        public void Delete()
        {
            MessageBoxResult res = MessageBox.Show("Are you sure you want to delete this game. This action is irreversible", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(res == MessageBoxResult.Yes)
            {
                try
                {
                    _fieldsaver.Delete(_selectedField.Id);
                }
                catch (ApplicationException ex)
                {
                    ShowErrorMessage(ex.Message, "Error while deleting field");
                }

                Fields.Remove(_selectedField);
                SelectedField = null;
            }
        }

        /// <summary>
        /// Overwrites the boats in the current field and replaces them with randomly generated ones.
        /// Switches to playing mode
        /// </summary>
        public void Generate()
        {
            MessageBoxResult res = MessageBox.Show("This will overwrite the current Field. Are you sure you want to confinue?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if(res == MessageBoxResult.Yes)
            {
                Field.GenerateBoats();
                ShowResult = false;
                EditMode = false;
                OnPropertyChanged(nameof(Field));
            }
        }

        /// <summary>
        /// Checks if a field is currently selected. Used for <see cref="ActionCommand.CanExecute(object)"/>
        /// </summary>
        /// <returns></returns>
        public bool IsFieldNotNull()
        {
            return Field != null;
        }

        /// <summary>
        /// This method is called, when the name of the selected <see cref="FieldSavingViewModel"/> changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        public void UpdateFieldName(object sender, PropertyChangedEventArgs ev)
        {
            if(ev.PropertyName == "Name")
            {
                Field.Name = _selectedField.Name;
            }
        }

        /// <summary>
        /// Toggles to the show results mode
        /// </summary>
        public void ShowRes()
        {
            ShowResult = true;
        }

        /// <summary>
        /// Switches between edit and playing mode
        /// </summary>
        public void ToggleEdit()
        {
            EditMode = !EditMode;
            ShowResult = false;
        }

        /// <summary>
        /// Should be called when a property of this object changes
        /// </summary>
        /// <param name="property"></param>
        private void OnPropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private ObservableCollection<FieldSavingViewModel> ConvertSavingDictToViewModel(Dictionary<Guid, string> dict)
        {
            return new ObservableCollection<FieldSavingViewModel>(
                dict.Select(d => new FieldSavingViewModel 
                { 
                    Id = d.Key,
                    Name = d.Value 
                }));
        }

        private void ShowErrorMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
