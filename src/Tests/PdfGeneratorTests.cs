using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BusinessLayer;
using BusinessLayer.DocumentGeneration;
using NUnit;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    class PdfGeneratorTests
    {
        /// <summary>
        /// This Test contains no asserts. It generates a Document,
        /// that must be evaluated manually. The Document is saved under 
        /// C:\Users\[username]\Documents\battleships_[timestamp].pdf
        /// </summary>
        [Test]
        public void TestGen()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"battleships_{DateTime.Now.Ticks}.pdf");
            File.Create(path).Close();

            Field field = new Field(7);
            field.CreateBoat(new Position(3, 3), new Position(3, 5));
            field.CreateBoat(new Position(0, 1), new Position(1, 1));
            field.CreateBoat(new Position(5, 6), new Position(5, 6));
            field.CreateBoat(new Position(5, 0), new Position(5, 3));
            field.CreateBoat(new Position(3, 1), new Position(3, 1));
            field.CreateBoat(new Position(2, 0), new Position(2, 0));

            PdfGenerator.Generate(path, field);

        }
    }
}
