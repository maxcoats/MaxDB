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

        public static bool IsDatabase(string name)
        {
            bool isDatabase = false;
            Database database = Databases.Where(s => s.Name == name).FirstOrDefault();

            if (database != null)
            {
                isDatabase = true;
            }

            return isDatabase;
        }

        public static void CreateDatabase(string name)
        {
            if (!IsDatabase(name))
            {
                Database database = new Database(name);
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
                Console.WriteLine("Failed to drop database " + name + "!");
            }
        }

        public static Database GetDatabase(string name)
        {
            Database database = Databases.Where(s => s.Name == name).FirstOrDefault();

            if (database == null)
            {
                Console.WriteLine("Failed to find database " + name + "!");
            }

            return database;
        }

        public static void ShowDatabases()
        {
            if (Databases.Count > 0)
            {
                Table table = ToTable();
                table.ToDisplayBuffer();
            }
            else
            {
                DatabaseEngine.DisplayBuffer.Add("No Databases found!");
            }

            DatabaseEngine.DisplayBufferToConsole();
        }

        public static Table ToTable()
        {
            Table table = new Table("Databases");
            table.CreateColumn("Database", "varchar", 255);
            List<string> fieldStrings = new List<string>();

            foreach (Database database in Databases)
            {
                fieldStrings.Add(database.Name);
            }

            foreach (string fieldString in fieldStrings)
            {
                foreach (Column column in table.Columns)
                {
                    Dictionary<string, string> fieldDictionary = new Dictionary<string, string>();
                    fieldDictionary.Add(column.Name, fieldString);
                    table.CreateRow(fieldDictionary);
                }
            }

            return table;
        }
    }
}
