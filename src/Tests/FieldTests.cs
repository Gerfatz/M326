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

            bool isNotDiagonal = (boatBits[0].XYPosition.X == boatBits[^1].XYPosition.X ||
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

        [Test]
        public void TestValidation()
        {
            Field field = new Field(7);

            field.CreateBoat(new Position(5, 2), new Position(3, 2));

            Assert.IsFalse(field.CreateBoat(new Position(5, 2), new Position(5, 4)));
            Assert.IsTrue(field.CreateBoat(new Position(1, 1), new Position(1, 1)));
        }

        [Test]
        public void TestGetBoatSizes()
        {
            Field field = new Field(7);

            //One Boat with length 1
            field.CreateBoat(new Position(0, 0), new Position(0, 0));

            //Two Boats with length 2
            field.CreateBoat(new Position(2, 0), new Position(2, 1));
            field.CreateBoat(new Position(0, 2), new Position(0, 3));


            //One Boat with length 4
            field.CreateBoat(new Position(4, 0), new Position(4, 3));

            Dictionary<int, int> res = field.GetBoatSizes();

            Assert.AreEqual(res[1], 1);
            Assert.AreEqual(res[2], 2);
            Assert.AreEqual(res[4], 1);
        }

        [Test]
        public void TestDeleteBoat()
        {
            Field field = new Field(7);

            field.CreateBoat(new Position(0, 0), new Position(0, 0));
            bool result = field.DeleteBoat(new Position(0, 0));

            Assert.AreEqual(field.Boats.Count, 0);
            Assert.IsTrue(result);
        }

        public void TestDeleteBoatFalsePosition()
        {
            Field field = new Field(7);

            field.CreateBoat(new Position(0, 0), new Position(0, 0));
            bool result = field.DeleteBoat(new Position(0, 1));

            Assert.AreEqual(field.Boats.Count, 1);
            Assert.IsFalse(result);
        }
    }
}
