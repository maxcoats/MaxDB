using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MaxDB.Utilities
{
    public static class StorageUtility
    {
        public static string DataFile { get; set; }

        static StorageUtility()
        {
            string path = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"Data\")).FullName;
            DataFile = Path.Combine(path, @"maxdb.json");
        }

        public static void WriteDatabaseCollectionToDisk(List<Database> databases)
        {
            File.WriteAllText(DataFile, JsonConvert.SerializeObject(databases));
        }

        public static List<Database> ReadDatabaseCollectionFromDisk()
        {
            List<Database> databases = new List<Database>();

            if (File.Exists(DataFile))
            {
                databases = JsonConvert.DeserializeObject<List<Database>>(File.ReadAllText(DataFile));
            }

            return databases;
        }
    }
}
