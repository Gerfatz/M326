using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private Dictionary<Guid, string> _fields;
        private Field _selectedField;
        private Guid _selectedFieldGuid;

        // Properties
        public bool? DeleteState { get; set; } = false; // State of "delete" checkbox
        public Field SelectedField {
            get => _selectedField;
            private set
            {
                _selectedField = value;
                OnPropertyChanged();
            }
        } // Created field from business layer

        public bool EditorMode { get; set; } = true; // State if editor mode is on
        public List<Position> SelectedPositions { get; set; } // List of positions selected by the user

        public Dictionary<Guid, string> Fields => _fields;

        //Commands
        public ActionCommand SaveCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Constructor
        public PlayingFieldViewModel()
        {
            SelectedPositions = new List<Position>();
            SaveCommand = new ActionCommand(Save);
            _fieldsaver = new Fieldsaver();
            _fields = _fieldsaver.GetAllFields();
        }

        public void Save()
        {
            _fieldsaver.Save(_selectedField, _selectedFieldGuid);
        }

        public void Load()
        {
            SelectedField = _fieldsaver.GetField(_selectedFieldGuid);
        }

        private void OnPropertyChanged([CallerMemberName]string property = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
