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
            return new Boat();
        }
    }
}
