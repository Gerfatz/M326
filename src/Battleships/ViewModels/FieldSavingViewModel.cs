using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BusinessLayer.Storage
{
    public class FieldSavingViewModel : INotifyPropertyChanged
    {
        private Guid _id;
        private string _name;

        public event PropertyChangedEventHandler PropertyChanged;



        public Guid Id 
        { 
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }    
        }

        public string Name 
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
