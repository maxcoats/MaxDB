using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxDB
{
    public class Table
    {
        public string Name { get; set; }

        public List<Column> Columns { get; set; }

        public List<Row> Rows { get; set; }

        public Table(string name)
        {
            Name = name;
            Columns = new List<Column>();
            Rows = new List<Row>();
        }

        public void CreateColumn(string name, string dataType, int size)
        {
            Column column = GetColumn(name);

            if (column == null)
            {
                column = new Column(name, dataType, size);
                Columns.Add(column);
            }
            else
            {
                Console.WriteLine("Failed to create column! A column named " + name + " already exists.");
            }
        }

        public void DropColumn(string name)
        {
            Column column = GetColumn(name);

            if (column != null)
            {
                Columns.Remove(column);
            }
            else
            {
                Console.WriteLine("Failed to drop column! A column named " + name + " could not be found.");
            }
        }

        public Column GetColumn(string name)
        {
            return Columns.Select(s => s).Where(s => s.Name == name).FirstOrDefault();
        }

        public void InsertRow()
        {

        }
    }
}
