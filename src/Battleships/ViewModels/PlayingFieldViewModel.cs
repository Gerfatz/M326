using System;
using System.Collections.Generic;
using System.Text;
using BusinessLayer;

namespace Battleships.ViewModels
{
    public class PlayingFieldViewModel
    {
        public int FieldSize { get; set; }
        public bool? ButtonState { get; set; } = false;
        public Field Field { get; set; }
        public bool EditorMode { get; set; } = true;
        public List<Position> SelectedPositions { get; set; }

        public PlayingFieldViewModel()
        {
            SelectedPositions = new List<Position>();
        }
    }
}
