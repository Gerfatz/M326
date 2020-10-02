using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BusinessLayer
{
    public class Boat
    {
        // Properties
        public List<BoatBit> BoatBits { get; set; }

        // Constructor
        private Boat()
        {
            
        }

        // Methods

        /// <summary>
        /// Checks if all boatsBits were found.
        /// </summary>
        /// <returns>Returns true when it was completely found, false if at least one bit has yet to be found.</returns>
        public bool WasFound()
        {
            foreach (BoatBit bit in this.BoatBits)
            {
                if (bit.WasFound == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Creates a new boat from two points on the field
        /// </summary>
        /// <param name="startPos">Start position of the boat</param>
        /// <param name="endPos">End position of the boat</param>
        /// <returns>Returns the created boat</returns>
        public Boat CreateBoat(Position startPos, Position endPos)
        {
            byte start;
            byte end;

            if (startPos.X == endPos.X)
            {
                if (startPos.Y > endPos.Y)
                {
                    start = endPos.Y;
                    end = startPos.Y;
                }
                else
                {
                    start = startPos.Y;
                    end = endPos.Y;
                }

                CreateBoatBits(start, end, endPos.X);
            }
            else if (startPos.Y == endPos.Y)
            {
                if (startPos.Y > endPos.Y)
                {
                    start = endPos.Y;
                    end = startPos.Y;
                }
                else
                {
                    start = startPos.Y;
                    end = endPos.Y;
                }
                CreateBoatBits(start, end, endPos.Y);
            }


            return this;
        }

        private void CreateBoatBits(byte start, byte end, byte axis)
        {
            byte difference;

            difference = (byte)(start - end);

            for (byte i = 0; i <= difference; i++)
            {
                Position pos = new Position(start, (byte)(axis + i));
                BoatBits.Add(new BoatBit(pos));
            }
        }
    }
}
