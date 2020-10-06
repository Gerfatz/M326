﻿using System;
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

        /// <summary>
        /// Creates the bits between two points on a given axis.
        /// </summary>
        /// <param name="startPos">First given position on axis</param>
        /// <param name="endPos">Last given position on axis</param>
        /// <param name="staticAxis">Common axis position</param>
        /// <param name="isXAxis">To Check if position is on the X axis</param>
        private void CreateBoatBits(byte startPos, byte endPos, byte staticAxis, bool isXAxis)
        {
            byte difference;
            byte start;
            byte end;

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
