using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxDB
{
    public static class DatabaseCollection
    {
        public static List<Database> Databases { get; set; }

        static DatabaseCollection()
        {
            Databases = new List<Database>();
        }

        public static void CreateDatabase(string name)
        {
            Database database = GetDatabase(name);

            if (database == null)
            {
                database = new Database(name);
                Databases.Add(database);
            }
            else
            {
                Console.WriteLine("Failed to create database! A database named " + name + " already exists.");
            }
        }

        public static void DropDatabase(string name)
        {
            Database database = GetDatabase(name);

            if (database != null)
            {
                Databases.Remove(database);
            }
            else
            {
                Console.WriteLine("Failed to drop database! A database named " + name + " could not be found.");
            }
        }

        public static Database GetDatabase(string name)
        {
            return Databases.Select(s => s).Where(s => s.Name == name).FirstOrDefault();
        }
    }
}
