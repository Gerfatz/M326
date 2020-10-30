using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace BusinessLayer.Storage
{
    public class Fieldsaver
    {

        private readonly string _folderName;

        private const string dictionaryFileName = "Dictionary.json";

        public Fieldsaver(): this("Battleships") { }

        public Fieldsaver(string foldername)
        {
            _folderName = foldername;
        }

        /// <summary>
        /// Returns a Dictionary with GUID as identifier and a string as value
        /// </summary>
        /// <remarks>
        /// The GUID can be used to access single fields. The string is the name of the field
        /// </remarks>
        public Dictionary<Guid, string> GetAllFields()
        {
            EnsureAppdataExists();

            string pathToFile = Path.Combine(PathToApplicationData, dictionaryFileName);

            using FileStream stream = new FileStream(pathToFile, FileMode.OpenOrCreate, FileAccess.Read);
            using StreamReader reader = new StreamReader(stream);

            return JsonConvert.DeserializeObject<Dictionary<Guid, string>>(reader.ReadToEnd()) ?? new Dictionary<Guid, string>();
        }

        /// <summary>
        /// Returns a <see cref="Field"/> with this id
        /// </summary>
        /// <param name="id">The GUID of the field</param>
        /// <remarks>
        /// in order to obtain the GUIDs of a field, call <see cref="GetAllFields"/>
        /// </remarks>
        /// 
        /// <exception cref="FileNotFoundException"/>
        public Field GetField(Guid id)
        {
            string pathToFile = GetPathForId(id);

            ThrowIfFileNotExists(pathToFile);

            using FileStream fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read);
            BinaryFormatter formatter = new BinaryFormatter();
            object data = formatter.Deserialize(fs);

            if (data is Field)
            {
                return data as Field;
            }
            else
            {
                throw new ApplicationException("The data in the file couldn't be deserialized, because the datatype is wrong");
            }
        }

        /// <summary>
        /// Saves a <see cref="Field"/> to a file
        /// </summary>
        /// <param name="data">The Field object</param>
        /// <param name="id">The id of the field</param>
        /// <remarks>
        /// The Field will be saved to a file with the name {id}.bin. If no file
        /// exists with this id, a new one will be created.
        /// </remarks>
        public void Save(Field data, Guid id)
        {
            EnsureAppdataExists();

            string pathToFile = GetPathForId(id);
            using FileStream fs = new FileStream(pathToFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(fs, data);

            UpdateDictionaryFile(id, data.Name);
        }

        /// <summary>
        /// Creates a new save for a field, returns the created GUID
        /// </summary>
        /// <param name="data">The <see cref="Field"/> to be saved</param>
        /// <returns></returns>
        public Guid Create(Field data)
        {
            Guid id = Guid.NewGuid();
            Save(data, id);
            return id;
        }

        /// <summary>
        /// Deletes the File
        /// </summary>
        /// <param name="guid">Id of the field</param>
        /// <exception cref="FileNotFoundException"/>
        public void Delete(Guid guid)
        {
            string pathToFile = GetPathForId(guid);
            ThrowIfFileNotExists(pathToFile);
            UpdateDictionaryFile(guid, null);

            File.Delete(pathToFile);
        }

        /// <summary>
        /// Updates the name of a field or creates a new entry in the dictionary file
        /// </summary>
        /// <param name="id">Id of the Field</param>
        /// <param name="name">New name</param>
        private void UpdateDictionaryFile(Guid id, string name)
        {
            string pathToFile = Path.Combine(PathToApplicationData, dictionaryFileName);

            Dictionary<Guid, string> fields = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(File.ReadAllText(pathToFile)) ?? new Dictionary<Guid, string>();

            if(name == null)
            {
                fields.Remove(id);
            }
            else
            {
                if (fields.ContainsKey(id))
                {
                    fields[id] = name;
                }
                else
                {
                    fields.Add(id, name);
                }
            }

            File.WriteAllText(pathToFile, JsonConvert.SerializeObject(fields));
        }

        /// <summary>
        /// returns the path of the applications appdata directory
        /// </summary>
        private string PathToApplicationData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _folderName);

        /// <summary>
        /// returns the filepath for a field
        /// </summary>
        /// <param name="id">id of the field</param>
        /// <remarks>The format is"{%appdata%}/local/battleships/{id}.bin</remarks>
        /// <returns></returns>
        private string GetPathForId(Guid id)
        {
            return Path.Combine(PathToApplicationData, id.ToString() + ".bin");
        }

        /// <summary>
        /// Checks if a File exists and throws an Exception if it doesnt
        /// </summary>
        /// <param name="path">Path to the File</param>
        /// <exception cref="FileNotFoundException"/>
        private void ThrowIfFileNotExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file with the name {path} could not be found", path);
            }
        }

        private void EnsureAppdataExists()
        {
            if (!Directory.Exists(PathToApplicationData))
            {
                Directory.CreateDirectory(PathToApplicationData);
            }
        }

    }
}
