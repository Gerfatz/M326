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
    public class FieldTests
    {
        [Test]
        public void TestValidBoat()
        {
            Field field = new Field(7);

            Position startPos = new Position(5, 2);
            Position endPos = new Position(3, 2);

            Assert.IsTrue(field.CreateBoat(startPos, endPos));

            Boat boat = field.Boats[0];

            Assert.NotNull(boat);

            List<BoatBit> boatBits = boat.BoatBits;

            bool isNotDiagonal = (boatBits[0].XYPosition.X == boatBits[boatBits.Count - 1].XYPosition.X ||
                boatBits[0].XYPosition.Y == boatBits[^1].XYPosition.Y) ;

            Assert.IsTrue(isNotDiagonal);
        }

        [Test]
        public void TestInvalidBoat()
        {
            Field field = new Field(7);

            Position startPos = new Position(5, 2);
            Position endPos = new Position(3, 4);

            Assert.IsFalse(field.CreateBoat(startPos, endPos));
            Assert.IsFalse(field.Boats.Any());
        }

        [Test]
        public void TestGenerationValid()
        {
            Field field = new Field(7);

            field.GenerateBoats();

            Assert.IsFalse(field.Boats.Any(x => field.ValidateBoatPos(x)));
        }
    }
}
