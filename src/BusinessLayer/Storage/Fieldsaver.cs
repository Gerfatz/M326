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
        /// <exception cref="ApplicationException"></exception>
        public Dictionary<Guid, string> GetAllFields()
        {
            //Ensuring the Folder exists
            EnsureFolderExists();

            string pathToFile = Path.Combine(PathToFolder, dictionaryFileName);

            //Reading and deserializing the contents of the file
            using FileStream stream = new FileStream(pathToFile, FileMode.OpenOrCreate, FileAccess.Read);
            using StreamReader reader = new StreamReader(stream);

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<Guid, string>>(reader.ReadToEnd()) ?? new Dictionary<Guid, string>();
            }
            catch
            {
                throw new ApplicationException("An error occured while loading the playing fields");
            }
        }

        /// <summary>
        /// Returns a <see cref="Field"/> with this id
        /// </summary>
        /// <param name="id">The GUID of the field</param>
        /// <remarks>
        /// in order to obtain the GUIDs of a field, call <see cref="GetAllFields"/>
        /// </remarks>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="ApplicationException"/>
        public Field GetField(Guid id)
        {
            string pathToFile = GetPathForId(id);

            //making sure the file and folder exists
            EnsureFolderExists();
            ThrowIfFileNotExists(pathToFile);

            //Reading and Deserializing
            using FileStream fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read);
            BinaryFormatter formatter = new BinaryFormatter();

            //Checking if the type is correct
            try
            {
                object data = formatter.Deserialize(fs);
                return data as Field;
            }
            catch
            {
                throw new ApplicationException("The data in the file couldn't be deserialized");
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
            string pathToFile = GetPathForId(id);

            //Ensuring Folder exists
            EnsureFolderExists();

            using FileStream fs = new FileStream(pathToFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            
            //Serializing
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
            UpdateDictionaryFile(guid, "", true);

            //If the File does not exist, we don't have to delete in anymore, nor throw an excepiton
            if (File.Exists(pathToFile))
            {
                File.Delete(pathToFile);
            }
        }

        /// <summary>
        /// Updates the name of a field or creates a new entry in the dictionary file
        /// </summary>
        /// <param name="id">Id of the Field</param>
        /// <param name="name">Name of the Field</param>
        /// <param name="delete">Should the entry with the id be deleted</param>
        private void UpdateDictionaryFile(Guid id, string name, bool delete = false)
        {
            string pathToFile = Path.Combine(PathToFolder, dictionaryFileName);

            //Ensuring the File exists
            if (!File.Exists(pathToFile))
            {
                File.Create(pathToFile).Close();
            }

            Dictionary<Guid, string> fields = null;

            try
            {
                //Loading current contents
                fields = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(File.ReadAllText(pathToFile)) ?? new Dictionary<Guid, string>();
            }
            catch (IOException)
            {
                throw new ApplicationException("A file is currently in use by another programm, the field might have not been saved correctly");
            }

            if (delete)
            {
                //Delete
                fields.Remove(id);
            }
            else
            {
                if (fields.ContainsKey(id))
                {
                    //Update
                    fields[id] = name;
                }
                else
                {
                    //Create
                    fields.Add(id, name);
                }
            }

            //Save to file
            File.WriteAllText(pathToFile, JsonConvert.SerializeObject(fields));
        }

        /// <summary>
        /// returns the path of the applications appdata directory
        /// </summary>
        private string PathToFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _folderName);

        /// <summary>
        /// returns the filepath for a field
        /// </summary>
        /// <param name="id">id of the field</param>
        /// <remarks>The format is"{%appdata%}/local/battleships/{id}.bin</remarks>
        /// <returns></returns>
        private string GetPathForId(Guid id)
        {
            return Path.Combine(PathToFolder, id.ToString() + ".bin");
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
                throw new ApplicationException("A file could not be found");
            }
        }

        private void EnsureFolderExists()
        {
            if (!Directory.Exists(PathToFolder))
            {
                Directory.CreateDirectory(PathToFolder);
            }
        }
    }
}
