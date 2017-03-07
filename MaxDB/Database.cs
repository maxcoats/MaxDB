using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxDB
{
    public class Database
    {
        public string Name { get; set; }

        public List<Table> Tables { get; set; }

        public Database(string name)
        {
            Name = name;
            Tables = new List<Table>();
        }

        public bool IsTable(string name)
        {
            bool isTable = false;
            Table table = Tables.Where(s => s.Name == name).FirstOrDefault();

            if (table != null)
            {
                isTable = true;
            }

            return isTable;
        }

        public void CreateTable(string name)
        {
            if (!IsTable(name))
            {
                Table table = new Table(name);
                Tables.Add(table);
            }
            else
            {
                Console.WriteLine("Failed to create table! A table named " + name + " already exists.");
            }
        }

        public void DropTable(string name)
        {
            Table table = GetTable(name);

            if (table != null)
            {
                Tables.Remove(table);
            }
            else
            {
                Console.WriteLine("Failed to drop table " + name + "!");
            }
        }

        public Table GetTable(string name)
        {
            Table table = Tables.Where(s => s.Name == name).FirstOrDefault();

            if (table == null)
            {
                Console.WriteLine("Failed to find table " + name + "!");
            }

            return table;
        }

        public void ShowTables()
        {
            if (Tables.Count > 0)
            {
                Table table = ToTable();
                table.ToDisplayBuffer();
            }
            else
            {
                DatabaseEngine.DisplayBuffer.Add("No Tables found!");
            }

            DatabaseEngine.DisplayBufferToConsole();
        }

        public Table ToTable()
        {
            Table table = new Table("Tables");
            table.CreateColumn("Table", "varchar", 255);
            List<string> dataItemStrings = new List<string>();

            foreach (Table itable in Tables)
            {
                dataItemStrings.Add(itable.Name);
            }

            foreach (string dataItemString in dataItemStrings)
            {
                foreach (Column column in table.Columns)
                {
                    Dictionary<string, string> dataItemDictionary = new Dictionary<string, string>();
                    dataItemDictionary.Add(column.Name, dataItemString);
                    table.CreateRow(dataItemDictionary);
                }
            }

            return table;
        }
    }
}
