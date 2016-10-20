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

        public void CreateTable(string name)
        {
            Table table = GetTable(name);

            if (table == null)
            {
                table = new Table(name);
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
                Console.WriteLine("Failed to drop table! A table named " + name + " could not be found.");
            }
        }

        public Table GetTable(string name)
        {
            return Tables.Select(s => s).Where(s => s.Name == name).FirstOrDefault();
        }
    }
}
