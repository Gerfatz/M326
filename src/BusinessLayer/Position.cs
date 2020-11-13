﻿using iText.Kernel.Pdf.Canvas.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer
{
    [Serializable]
    public struct Position : IEquatable<Position>
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

        public bool Equals(Position pos)
        {
            return this == pos;
        }

        public static bool operator ==(Position pos1, Position pos2)
        {
            return pos1.X == pos2.X && pos1.Y == pos2.Y;
        }

        public static bool operator !=(Position pos1, Position pos2)
        {
            return pos1.X != pos2.X || pos2.Y == pos1.Y;
        }
    }
}
