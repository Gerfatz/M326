using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer
{
    [Serializable]
    public struct Position
    {
        // Properties
        public byte X { get; set; }
        public byte Y { get; set; }

        // Constructor
        public Position(byte x, byte y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
