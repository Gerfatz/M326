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

            if (boat == null || !ValidateBoatPos(boat))
                return false;

            _boats.Add(boat);
            return true;
        }

        public bool ValidateBoatPos(Boat boat)
        {
            List<Position> boatPositions = new List<Position>();
            foreach (BoatBit boatBit in boat.BoatBits)
            {
                for (int Y = -1; Y <= 1; Y++)
                {
                    for (int X = -1; X <= 1; X++)
                    {
                        sbyte posX = Convert.ToSByte(boatBit.XYPosition.X + X);
                        sbyte posY = Convert.ToSByte(boatBit.XYPosition.Y + Y);
                        Position position = new Position(posX, posY);
                        boatPositions.Add(position);
                    }
                }
            }

            foreach (Boat loopBoat in _boats)
            {
                foreach (BoatBit boatBit in loopBoat.BoatBits)
                {
                    Position bitPos = boatBit.XYPosition;
                    Position pos = boatPositions.Find(x => x.X == bitPos.X && x.Y == bitPos.Y);
                    
                    if (boatPositions.Exists(x => x.X == bitPos.X && x.Y == bitPos.Y))
                        return false;
                }
            }

            return true;
        }

        public int SideLength => _sideLength;

        public List<Boat> Boats => _boats;
    }
}
