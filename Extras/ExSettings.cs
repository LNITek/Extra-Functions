using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExtraFunctions.Extras
{
    /// <summary>
    /// Helps Manage Your Programs Settings Via A Exturnal File
    /// </summary>
    public class ExSettings
    {
        List<FileType> SupportedTypes = new() { FileType.txt, FileType.dat };
        Dictionary<string, string> Settings = new();
        /// <summary>
        /// The Full Path To The Settings File
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Create A New Settings File Or Load In An Existing One
        /// </summary>
        /// <param name="SaveDir">The Directory Of The New Settings File</param>
        /// <param name="FileExtension">The File Extension To Use</param>
        public ExSettings(string SaveDir, FileType FileExtension = FileType.dat)
        {
            if (!SupportedTypes.Contains(FileExtension))
                throw new FileFormatException(FileExtension.ToString() + " : Format Is Not Supported");
            Directory.CreateDirectory(SaveDir);
            Path = SaveDir + "\\Settings." + FileExtension.ToString();
            if (File.Exists(Path)) Load();
        }

        /// <summary>
        /// Loads In Settings File
        /// </summary>
        /// <param name="Dir">The Full Path Of The Settings File</param>
        public ExSettings(string Dir)
        {
            Path = Dir;
            Load();
        }
        /// <summary>
        /// Gets The Setting Value
        /// </summary>
        /// <param name="Name">Setting Name</param>
        /// <returns>The Specified Setting Value</returns>
        /// <exception cref="Exception">Fails If Setting Does Not Exist</exception>
        public string GetValue(string Name)
        {
            if (!Settings.ContainsKey(Name)) throw new Exception($"Setting ({Name}) Does Not Exist");
            return Settings[Name];
        }
        /// <summary>
        /// Gets All Setting Names
        /// </summary>
        /// <returns>All Setting Names</returns>
        public string[] GetNames() => Settings.Keys.ToArray();
        /// <summary>
        /// Gets All Of The Entire Setting
        /// </summary>
        /// <returns>All Of The Settings</returns>
        public IEnumerable<KeyValuePair<string, string>> GetSettings() => Settings;
        /// <summary>
        /// Test If A Setting Already Exists
        /// </summary>
        /// <param name="Name">Name Of The Setting</param>
        /// <returns>True If It Exist, Otherwise False</returns>
        public bool Contains(string Name) => Settings.ContainsKey(Name); 

        /// <summary>
        /// Remove A Setting
        /// </summary>
        /// <param name="Name">The Name Of The Setting</param>
        /// <exception cref="Exception">Fails If Setting Does Not Exist</exception>
        public void Remove(string Name)
        {
            if (!Settings.ContainsKey(Name)) throw new Exception($"Setting ({Name}) Does Not Exist");
            Settings.Remove(Name);
        }
        /// <summary>
        /// Remove A Setting By Index
        /// </summary>
        /// <param name="Index">The Index Of The Setting</param>
        /// <exception cref="Exception">Fails If Index Is Outside Of The Total Number Of Settings Or Invalid</exception>
        public void RemoveAt(int Index)
        {
            if (Index < 0) throw new Exception("Index Is Outside Of Bounds : " + Index);
            if (Index > Settings.Count) throw new Exception("Index Is Outside Of Bounds : " + Index);
            Settings.Remove(Settings.ElementAt(Index).Key);
        }
        /// <summary>
        /// Removes All Settings That Contains The Value
        /// </summary>
        /// <param name="Value">The Value To Remove</param>
        public void RemoveAll(string Value) => 
            Settings.Where(x => x.Key.Contains(Value)).ToList().ForEach(x => Settings.Remove(x.Key));
        /// <summary>
        /// Reverses The Settings Object
        /// </summary>
        public void Reverse() => Settings.Reverse();

        /// <summary>
        /// Adds A New Setting
        /// </summary>
        /// <param name="Name">The Name Of The Setting</param>
        /// <param name="Value">The Value Of The Setting</param>
        /// <exception cref="Exception">Fails If The Setting Exists</exception>
        public void Add(string Name, string Value)
        {
            if (Settings.ContainsKey(Name)) throw new Exception(Name + " : Allready Exists");
            Settings.Add(Name, Value);
        }
        /// <summary>
        /// Adds A New Setting
        /// </summary>
        /// <param name="Item">Adds A New Setting</param>
        /// <exception cref="Exception">Fails If The Setting Exists</exception>
        public void Add(KeyValuePair<string, string> Item)
        {
            if (Settings.ContainsKey(Item.Key)) throw new Exception(Item.Key + " : Allready Exists");
            Settings.Add(Item.Key, Item.Value);
        }
        /// <summary>
        /// Adds A List Of New Settings
        /// </summary>
        /// <param name="Items">The List Of The New Settings Names And Values</param>
        /// <exception cref="Exception">Fails If One Of The Setting Exists</exception>
        public void AddRange(IEnumerable<KeyValuePair<string, string>> Items) =>
            Items.ToList().ForEach(x =>
            {
                if (Settings.ContainsKey(x.Key)) throw new Exception(x.Key + " : Allready Exists");
                Settings.Add(x.Key, x.Value);
            });

        /// <summary>
        /// Set A New Value To A Setting
        /// </summary>
        /// <param name="Name">Setting Name</param>
        /// <param name="Value">New Value</param>
        /// <exception cref="Exception">Fails If The Setting Does Not Exists</exception>
        public void SetSetting(string Name, string Value)
        {
            if (!Settings.ContainsKey(Name)) throw new Exception(Name + " : Does Not Exists");
            Settings[Name] = Value;
        }
        /// <summary>
        /// Rename A Setting
        /// </summary>
        /// <param name="OldName">Old Setting Name</param>
        /// <param name="NewName">New Setting Name</param>
        /// <exception cref="Exception">Fails If The Setting Does Not Exists</exception>
        public void Rename(string OldName, string NewName)
        {
            if (!Settings.ContainsKey(OldName)) throw new Exception(OldName + " : Does Not Exists");
            var V = Settings[OldName];
            Remove(OldName);
            Add(NewName, V);
        }

        /// <summary>
        /// Saves The Settings To The File
        /// </summary>
        public void Save()
        {
            StreamWriter Writer = new(Path, false);
            foreach(var Item in Settings)
                Writer.WriteLine($"{Item.Key}={Item.Value}");
            Writer.Close();
        }

        /// <summary>
        /// Loads The Settings From The File
        /// </summary>
        /// <returns>The Hole Settings Object</returns>
        public IEnumerable<KeyValuePair<string, string>> Load()
        {
            if (!File.Exists(Path)) Save();
            StreamReader Reader = new(Path);
            while (!Reader.EndOfStream)
            {
                var Item = Reader.ReadLine();
                if (string.IsNullOrWhiteSpace(Item)) continue;
                var Values = Item.Split('=');
                Add(Values.First(), Values.Last());
            }
            Reader.Close();
            return Settings;
        }
    }
}
