using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Battleships.Commands;
using BusinessLayer;
using BusinessLayer.Storage;

namespace Battleships.ViewModels
{
    public class PlayingFieldViewModel : INotifyPropertyChanged
    {
        private readonly Fieldsaver _fieldsaver;
        private Field _field;
        private FieldSavingModel _selectedField;
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

        public ObservableCollection<FieldSavingModel> Fields { get; private set; }

        public FieldSavingModel SelectedField
        {
            get => _selectedField;
            set
            {
                _selectedField = value;
                OnPropertyChanged();
            }
        }

        [Range(5, 20)]
        public int SideLength
        {
            get => _sideLength;
            set
            {
                _sideLength = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
                OnPropertyChanged(nameof(GameMode));
            }
        }

        public bool GameMode => !_editMode;

        //Commands
        public ActionCommand SaveCommand { get; set; }
        public ActionCommand LoadCommand { get; set; }
        public ActionCommand NewFieldCommand { get; set; }
        public ActionCommand ShowResultCommand { get; set; }
        public ActionCommand ToggleEditCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Constructor
        public PlayingFieldViewModel()
        {
            SelectedPositions = new List<Position>();

            SaveCommand = new ActionCommand(Save);
            LoadCommand = new ActionCommand(Load);
            NewFieldCommand = new ActionCommand(NewField);
            ShowResultCommand = new ActionCommand(ShowRes);
            ToggleEditCommand = new ActionCommand(ToggleEdit);

            _fieldsaver = new Fieldsaver();
            Fields = new ObservableCollection<FieldSavingModel>(_fieldsaver.GetAllFields());
        }

        public void Save()
        {
            _fieldsaver.Save(_field, _selectedField.Id);
        }

        public void Load()
        {
            Field = _fieldsaver.GetField(_selectedField.Id);
        }

        public void NewField()
        {
            Field field = new Field(_sideLength);
            Guid newFieldGuid = _fieldsaver.Create(field);
            Fields.Add(new FieldSavingModel
            { 
                Id = newFieldGuid,
                Name = "New Field"
            });

            OnPropertyChanged(nameof(Fields));

            SelectedField = Fields.First(f => f.Id == newFieldGuid);
            Field = field;
        }

        public void ShowRes()
        {
            ShowResult = true;
        }

        public void ToggleEdit()
        {
            EditMode = !EditMode;
        }

        private void OnPropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
