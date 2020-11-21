using System;
using System.Collections.Generic;
using System.Text;
using BusinessLayer;

namespace Battleships.ViewModels
{
    public class PlayingFieldViewModel
    {
        public int FieldSize { get; set; }
        public Field Field { get; set; }
    }
}
