using System;

namespace BusinessLayer
{
    public class BoatBit
    {
        // Properties
        public bool WasFound { get; set; }
        public Position XYPosition { get; set; }

        // Constructor
        public BoatBit(Position position, bool wasFound = false)
        {
            this.WasFound = wasFound;
            this.XYPosition = position;
        }
    }
}
