using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
        private FieldSavingModel _selectedField;
        private ObservableCollection<FieldSavingModel> _fields;
        private int _sideLength = 7;
        private bool _showResult = false;
        private bool _editMode = true;

        // Properties
        public bool? DeleteState { get; set; } = false; // State of "delete" checkbox
        public Field Field {
            get => _field;
            private set
            {
                _field = value;
                OnPropertyChanged();
            }
        } // Created field from business layer

        public List<Position> SelectedPositions { get; set; } // List of positions selected by the user

        public ObservableCollection<FieldSavingModel> Fields 
        {
            get => _fields;
            private set
            {
                _fields = value;
                OnPropertyChanged();
            }
        }

        public FieldSavingModel SelectedField
        {
            get => _selectedField;
            set
            {
                if(_selectedField != null)
                {
                    _selectedField.PropertyChanged -= UpdateFieldName;
                }
                _selectedField = value;

                if(value != null)
                {
                    _selectedField.PropertyChanged += UpdateFieldName;
                    Field = _fieldsaver.GetField(_selectedField.Id);
                }
                else
                {
                    Field = null;
                }

                OnPropertyChanged();
            }
        }

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

        public bool ShowResult
        {
            get => _showResult;
            private set
            {
                _showResult = value;
                OnPropertyChanged();
            }
        }

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

        public bool GameMode => !_editMode;

        //Commands
        public ActionCommand SaveCommand { get; set; }
        public ActionCommand NewFieldCommand { get; set; }
        public ActionCommand ShowResultCommand { get; set; }
        public ActionCommand ToggleEditCommand { get; set; }
        public ActionCommand ExportCommand { get; set; }
        public ActionCommand DeleteCommand { get; set; }
        public ActionCommand GenerateCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Constructor
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
            Fields = new ObservableCollection<FieldSavingModel>(_fieldsaver.GetAllFields());
        }

        public void Save()
        {
            _fieldsaver.Save(_field, _selectedField.Id);
        }

        public void NewField()
        {
            Field field = new Field(_sideLength);
            field.Name = "New Field";
            Guid newFieldGuid = _fieldsaver.Create(field);
            Fields = new ObservableCollection<FieldSavingModel>(_fieldsaver.GetAllFields());

            SelectedField = Fields.First(f => f.Id == newFieldGuid);
            Field = field;
        }

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

        public void Delete()
        {
            MessageBoxResult res = MessageBox.Show("Are you sure you want to delete this game. This action is irreversible", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(res == MessageBoxResult.Yes)
            {
                _fieldsaver.Delete(_selectedField.Id);
                Fields.Remove(_selectedField);
                SelectedField = null;
            }
        }

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

        public bool IsFieldNotNull()
        {
            return Field != null;
        }

        public void UpdateFieldName(object sender, PropertyChangedEventArgs ev)
        {
            if(ev.PropertyName == "Name")
            {
                Field.Name = _selectedField.Name;
            }
        }

        public void ShowRes()
        {
            ShowResult = true;
        }

        public void ToggleEdit()
        {
            EditMode = !EditMode;
            ShowResult = false;
        }

        private void OnPropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
