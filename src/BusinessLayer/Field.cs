using System;
using System.Collections.Generic;
using System.Text;
using BusinessLayer;

namespace BusinessLayer
{
    public class Field
    {
        // Properties
        public List<Boat> Boats { get; set; }
        public string Name { get; set; }

        // Attributes
        private int _sideLength;

        // Constructor
        public Field(int sideLength)
        {
            this._sideLength = sideLength;
        }

        // Methods

        /// <summary>
        /// Creates a boat on the field
        /// </summary>
        /// <param name="startPos">first position of boat</param>
        /// <param name="endPos">last position of boat</param>
        /// <returns>True if creation of boat was successfull, else false</returns>
        public bool CreateBoat(Position startPos, Position endPos)
        {
            Boat boat = Boat.CreateBoat(startPos, endPos);

            if (boat == null)
                return false;

            Boats.Add(boat);
            return true;
        }

        public int GetSideLength()
        {
            return this._sideLength;
        }

        public List<Boat> GetBoats()
        {
            return this.Boats;
        }


    }
}
