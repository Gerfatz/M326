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
    public class FieldsaverTests
    {
        private const string folder = "Battleships_test";

        [Test]
        public void TestGetAndCreate()
        {
            Fieldsaver fieldsaver = new Fieldsaver(folder);

            Field field = new Field(5)
            {
                Name = "Create_Test"
            };

            Guid id = fieldsaver.Create(field);

            Field saved = fieldsaver.GetField(id);

            Assert.AreEqual(field.Name, saved.Name);
            Assert.AreEqual(field.SideLength, saved.SideLength);
        }

        [Test]
        public void TestGetAll()
        {
            Fieldsaver fieldsaver = new Fieldsaver(folder);

            Field field = new Field(5)
            {
                Name = "GetAll_Test_1"
            };

            Guid id1 = fieldsaver.Create(field);

            field = new Field(7)
            {
                Name = "GetAll_Test_2"
            };

            Guid id2 = fieldsaver.Create(field);

            List<FieldSavingModel> models = fieldsaver.GetAllFields();

            Assert.IsTrue(models.Any(m => m.Id == id1) && models.Any(m => m.Id == id2));
            Assert.IsTrue(models.First(m => m.Id == id1).Name == "GetAll_Test_1");
            Assert.IsTrue(models.First(m => m.Id == id2).Name == "GetAll_Test_2");
        }

        [Test]
        public void TestUpdate()
        {
            Fieldsaver fieldsaver = new Fieldsaver(folder);

            Field field = new Field(7)
            {
                Name = "Update_Test_Old"
            };

            Guid fieldId = fieldsaver.Create(field);

            field.Name = "Update_Test_New";

            fieldsaver.Save(field, fieldId);
            field = fieldsaver.GetField(fieldId);

            Assert.AreEqual("Update_Test_New", field.Name);
        }

        [Test]
        public void TestDelete()
        {
            Fieldsaver fieldsaver = new Fieldsaver(folder);

            Field field = new Field(7)
            {
                Name = "Delete_Test"
            };

            Guid fieldId = fieldsaver.Create(field);

            fieldsaver.Delete(fieldId);
            Assert.Throws(typeof(FileNotFoundException), () => fieldsaver.GetField(fieldId));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), folder);
            Directory.EnumerateFiles(folderPath).ToList().ForEach(f => File.Delete(f));
            Directory.Delete(folderPath);
        }
    }
}
