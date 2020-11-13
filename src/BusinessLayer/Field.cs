using System;
using System.Collections.Generic;
using System.Text;
using BusinessLayer;

namespace BusinessLayer
{
    [Serializable]
    public class Field
    {
        // Properties
        private List<Boat> _boats { get; set; }
        public string Name { get; set; }

        // Attributes
        private int _sideLength;

        // Constructor
        public Field(int sideLength)
        {
            _sideLength = sideLength;
            _boats = new List<Boat>();
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

            _boats.Add(boat);
            return true;
        }

        public int SideLength => _sideLength;

        public List<Boat> Boats => _boats;
    }
}
