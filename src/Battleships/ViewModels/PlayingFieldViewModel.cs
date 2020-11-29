using System;
using System.Collections.Generic;
using System.Text;
using BusinessLayer;

namespace Battleships.ViewModels
{
    public class PlayingFieldViewModel
    {
        // Properties
        public bool? DeleteState { get; set; } = false; // State of "delete" checkbox
        public Field Field { get; set; } // Created field from business layer
        public bool EditorMode { get; set; } = true; // State if editor mode is on
        public List<Position> SelectedPositions { get; set; } // List of positions selected by the user

        // Constructor
        public PlayingFieldViewModel()
        {
            SelectedPositions = new List<Position>();
        }
    }
}
