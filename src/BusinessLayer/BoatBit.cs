using System;
using System.Linq;

namespace BusinessLayer
{
    [Serializable]
    public class BoatBit
    {
        // Properties
        public bool WasFound { get; set; }
        public Position XYPosition { get; private set; }

        public Boat Boat { get; private set; }

        // Constructor
        public BoatBit(Position position, Boat boat, bool wasFound = false)
        {
            this.WasFound = wasFound;
            this.XYPosition = position;
            Boat = boat;
        }

        public BoatTile GetBoatBitType()
        {
            if(Boat.BoatBits.Count == 1)
            {
                return BoatTile.Single;
            }

            bool isOnXAxis = Boat.BoatBits.Count(bb => bb.XYPosition.X == XYPosition.X) == 1;

            if (isOnXAxis)
            {
                if(Boat.BoatBits.Max(bb => bb.XYPosition.X) == XYPosition.X)
                {
                    return BoatTile.Right;
                }
                else if(Boat.BoatBits.Min(bb => bb.XYPosition.X) == XYPosition.X)
                {
                    return BoatTile.Left;
                }
            }
            else
            {
                if(Boat.BoatBits.Max(bb => bb.XYPosition.Y) == XYPosition.Y)
                {
                    return BoatTile.Bottom;
                }
                else if (Boat.BoatBits.Min(bb => bb.XYPosition.Y) == XYPosition.Y)
                {
                    return BoatTile.Top;
                }
            }

            return BoatTile.Center;
        }
    }

    public enum BoatTile
    {
        Center,
        Top,
        Bottom,
        Right,
        Left,
        Single
    }
}
