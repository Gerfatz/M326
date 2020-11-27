using System;
using System.Collections.Generic;
using System.Text;
using BusinessLayer;

namespace Battleships.ViewModels
{
    public class PlayingFieldViewModel
    {
        public int FieldSize { get; set; }
        public bool? ButtonState { get; set; }
        public Field Field { get; set; }
        public bool EditorMode { get; set; } = true;
    }
}
