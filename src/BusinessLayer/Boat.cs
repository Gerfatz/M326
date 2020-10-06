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
            if (startPos.X == endPos.X)
            {
                CreateBoatBits(startPos.Y, endPos.Y, endPos.X, true);
            }
            else if (startPos.Y == endPos.Y)
            {
                CreateBoatBits(startPos.X, endPos.X, endPos.Y, false);
            }
            else
            {
                return null;
            }


            return this;
        }

        private void CreateBoatBits(byte startCord, byte endCord, byte staticAxis, bool isXAxis)
        {
            byte difference;
            byte start;
            byte end;

            if (startCord > endCord)
            {
                start = startCord;
                end = endCord;
            }
            else
            {
                start = endCord;
                end = startCord;
            }


            difference = (byte)(start - end);

            for (byte i = 0; i <= difference; i++)
            {
                if (isXAxis)
                {
                    Position pos = new Position(staticAxis, (byte)(end + i));
                    this.BoatBits.Add(new BoatBit(pos));
                }
                else
                {
                    Position pos = new Position((byte)(end + i), staticAxis);
                    this.BoatBits.Add(new BoatBit(pos));
                }
            }
        }
    }
}
