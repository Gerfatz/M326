using iText.Kernel.Pdf.Canvas.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer
{
    [Serializable]
    public struct Position : IEquatable<Position>
    {
        // Properties
        public sbyte X { get; set; }
        public sbyte Y { get; set; }

        // Constructor
        public Position(sbyte x, sbyte y)
        {
            this.X = x;
            this.Y = y;
        }

        public static bool operator ==(Position pos1, Position pos2)
        {
            return pos1.X == pos2.X && pos1.Y == pos2.Y;
        }

        public static bool operator !=(Position pos1, Position pos2)
        {
            return pos1.X != pos2.X || pos2.Y == pos1.Y;
        }

        public bool Equals(Position pos)
        {
            return pos == this;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return X + (Y << 8);
        }
    }
}
