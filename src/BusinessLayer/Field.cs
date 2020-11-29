﻿using System;
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
        private List<Boat> _boats { get; set; } // List of all boats in field
        public string Name { get; set; } // Name of field for saving (might be legacy now...)

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

            Random r = new Random();
            int randUncover = r.Next(3);

            if (randUncover == 1)
            {
                boat.UncoverBoat();
            }

            return true;
        }

        /// <summary>
        /// Checks if boats position is already occupied or 
        /// otherwise invalid based on the existing boats in _boats
        /// </summary>
        /// <param name="boat">Boat that is to be validated</param>
        /// <returns>If the boats position is valid or not</returns>
        private bool ValidateBoatPos(Boat boat)
        {
            List<Position> boatPositions = new List<Position>();
            foreach (BoatBit boatBit in boat.BoatBits)
            {
                // Adds positions around and in "boatBit" to "boatPositions"
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

            // Checks through all BoatBits in "_boats"
            foreach (BoatBit boatBit in _boats.SelectMany(b => b.BoatBits))
            {
                Position bitPos = boatBit.XYPosition;
                Position pos = boatPositions.Find(x => x.X == bitPos.X && x.Y == bitPos.Y);

                // Checks if position is already occupied in "_boats"
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

        /// <summary>
        /// Returns amount of BoatBits on x coordinate
        /// </summary>
        /// <param name="yCoord">X coordinate</param>
        /// <returns>Amount of BoatBits in xCoord</returns>
        public int XBoatCount(sbyte xCoord)
        {
            int count = 0;
            foreach (Boat boat in _boats)
            {
                count += boat.BoatBits.Count(x => x.XYPosition.X == xCoord);
            }

            return count;
        }

        /// <summary>
        /// Returns amount of BoatBits on y coordinate
        /// </summary>
        /// <param name="yCoord">Y coordinate</param>
        /// <returns>Amount of BoatBits in yCoord</returns>
        public int YBoatCount(sbyte yCoord)
        {
            int count = 0;
            foreach (Boat boat in _boats)
            {
                count += boat.BoatBits.Count(x => x.XYPosition.Y == yCoord);
            }

            return count;
        }


        public void GenerateBoats()
        {
            _boats.Clear();

            List<Position> availablePositions = new List<Position>();

            for (sbyte y = 0; y < _sideLength; y++)
            {
                for (sbyte x = 0; x < _sideLength; x++)
                {
                    Position pos = new Position(x, y);
                    availablePositions.Add(pos);
                }
            }

            Random random = new Random();
            while (availablePositions.Count > 0)
            {
                Position pos1 = availablePositions[random.Next(availablePositions.Count)];
                Position pos2 = availablePositions[random.Next(availablePositions.Count)];
                if (CreateBoat(pos1, pos2))
                {
                    Boat boat = _boats[_boats.Count - 1];
                    foreach (BoatBit boatBit in boat.BoatBits)
                    {
                        // Adds positions around and in "boatBit" to "boatPositions"
                        for (int y = -1; y <= 1; y++)
                        {
                            for (int x = -1; x <= 1; x++)
                            {
                                sbyte posX = Convert.ToSByte(boatBit.XYPosition.X + x);
                                sbyte posY = Convert.ToSByte(boatBit.XYPosition.Y + y);
                                Position position = new Position(posX, posY);
                                availablePositions.Remove(position);
                            }
                        }
                    }
                }
            }
        }


        public int SideLength => _sideLength;

        public List<Boat> Boats => _boats;
    }
}
