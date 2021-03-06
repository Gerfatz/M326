﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace BusinessLayer
{
    [Serializable]
    public class Boat
    {
        // Properties
        public List<BoatBit> BoatBits { get; set; }

        // Constructor
        private Boat()
        {
            BoatBits = new List<BoatBit>();
        }

        // Methods

        /// <summary>
        /// Checks if all boatsBits were found.
        /// </summary>
        /// <returns>Returns true when it was completely found, false if at least one bit has yet to be found.</returns>
        public bool WasFound => BoatBits.All(bb => bb.WasFound);


        /// <summary>
        /// Creates a new boat from two points on the field
        /// </summary>
        /// <param name="startPos">Start position of the boat</param>
        /// <param name="endPos">End position of the boat</param>
        /// <returns>Returns the created boat</returns>
        public static Boat CreateBoat(Position startPos, Position endPos)
        {
            Boat boat = new Boat();

            if (startPos.X == endPos.X)
            {
                boat.CreateBoatBits(startPos.Y, endPos.Y, endPos.X, true);
            }
            else if (startPos.Y == endPos.Y)
            {
                boat.CreateBoatBits(startPos.X, endPos.X, endPos.Y, false);
            }
            else
            {
                return null;
            }


            return boat;
        }

        /// <summary>
        /// Sets all BoatBits in boat to "found"
        /// </summary>
        public void UncoverBoat()
        {
            foreach (BoatBit boatBit in BoatBits)
            {
                boatBit.WasFound = true;
            }
        }

        /// <summary>
        /// Creates the bits between two points on a given axis.
        /// </summary>
        /// <param name="startPos">First given position on axis</param>
        /// <param name="endPos">Last given position on axis</param>
        /// <param name="staticAxis">Common axis position</param>
        /// <param name="isXAxis">To Check if position is on the X axis</param>
        private void CreateBoatBits(sbyte startPos, sbyte endPos, sbyte staticAxis, bool isXAxis)
        {
            sbyte difference;
            sbyte start;
            sbyte end;
            Position pos;

            if (startPos > endPos)
            {
                start = startPos;
                end = endPos;
            }
            else
            {
                start = endPos;
                end = startPos;
            }


            difference = (sbyte)(start - end);

            for (sbyte i = 0; i <= difference; i++)
            {
                if (isXAxis)
                {
                    pos = new Position(staticAxis, (sbyte)(end + i));
                }
                else
                {
                    pos = new Position((sbyte)(end + i), staticAxis);
                }
                BoatBits.Add(new BoatBit(pos, this));
            }
        }
    }
}
