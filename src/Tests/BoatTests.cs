using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using BusinessLayer.Storage;
using System.IO;
using BusinessLayer;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class BoatTests
    {
        [Test]
        public void TestValidBoat()
        {
            Position startPos = new Position(5, 2);
            Position endPos = new Position(3, 2);

            Boat boat = Boat.CreateBoat(startPos, endPos);

            List<BoatBit> boatBits = boat.BoatBits;

            bool isNotDiagonal = (boatBits[0].XYPosition.X == boatBits[^1].XYPosition.X ||
                boatBits[0].XYPosition.Y == boatBits[^1].XYPosition.Y);

            Assert.IsTrue(isNotDiagonal);
        }

        [Test]
        public void TestInvalidBoat()
        {
            Position startPos = new Position(5, 2);
            Position endPos = new Position(3, 4);

            Boat boat = Boat.CreateBoat(startPos, endPos);

            Assert.IsNull(boat);
        }
    }
}
