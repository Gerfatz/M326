using System;
using System.Collections.Generic;
using System.Linq;
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

        private bool ValidateBoatPos(Boat boat)
        {
            List<Position> boatPositions = new List<Position>();
            foreach (BoatBit boatBit in boat.BoatBits)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        sbyte posX = Convert.ToSByte(boatBit.XYPosition.X + x);
                        sbyte posY = Convert.ToSByte(boatBit.XYPosition.Y + y);
                        Position position = new Position(posX, posY);
                        boatPositions.Add(position);
                    }
                }
            }

            foreach (BoatBit boatBit in _boats.SelectMany(b => b.BoatBits))
            {
                Position bitPos = boatBit.XYPosition;
                Position pos = boatPositions.Find(x => x.X == bitPos.X && x.Y == bitPos.Y);
                    
                if (boatPositions.Exists(x => x.X == bitPos.X && x.Y == bitPos.Y))
                    return false;
            }

            return true;
        }


        // REMINDER: Change after Darios push from his home PC
        public bool DeleteBoat(Position position)
        {
            //BoatBit boatBit = _boats.SelectMany(x => x.BoatBits).FirstOrDefault(x => x.XYPosition == position);
            Boat boat = _boats.FirstOrDefault(x => x.BoatBits.Any(y => y.XYPosition == position));

            if (boat == null)
            {
                return false;
            }

            _boats.Remove(boat);
            return true;
        }

        public int XBoatCount(sbyte xCoord)
        {
            int count = 0;
            foreach (Boat boat in _boats)
            {
                count += boat.BoatBits.Count(x => x.XYPosition.X == xCoord);
            }

            return count;
        }

        public int YBoatCount(sbyte yCoord)
        {
            int count = 0;
            foreach (Boat boat in _boats)
            {
                count += boat.BoatBits.Count(x => x.XYPosition.Y == yCoord);
            }

            return count;
        }

        public int SideLength => _sideLength;

        public List<Boat> Boats => _boats;
    }
}
