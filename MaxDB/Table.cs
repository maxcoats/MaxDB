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

        public Table Copy()
        {
            Table table = new Table(Name);

            foreach (Column column in Columns)
            {
                table.CreateColumn(column.Name, column.DataType, column.Size);
            }

            foreach (Row row in Rows)
            {
                Dictionary<string, string> fieldDictionary = new Dictionary<string, string>();

                foreach (Column column in Columns)
                {
                    fieldDictionary.Add(column.Name, row.GetField(column).Data);
                }

                table.CreateRow(fieldDictionary);
            }

            return table;
        }

        public bool IsColumn(string name)
        {
            bool isColumn = false;
            Column column = Columns.Where(s => s.Name == name).FirstOrDefault();

            if (column != null)
            {
                isColumn = true;
            }

            return isColumn;
        }

        public void CreateColumn(string name, string dataType, int size)
        {
            if (!IsColumn(name))
            {
                Column column = new Column(name, dataType, size);
                Columns.Add(column);
            }
            else
            {
                Console.WriteLine("Failed to create column! A column named " + name + " already exists.");
            }
        }

        public void DropColumn(Column column)
        {
            if (column != null)
            {
                foreach (Row row in Rows)
                {
                    row.DropField(column);
                }

                Columns.Remove(column);
            }
            else
            {
                Console.WriteLine("Failed to drop column!");
            }
        }

        public void DropColumns(List<Column> columns)
        {
            bool firstIteration = true;

            foreach (Row row in Rows)
            {
                foreach (Column column in columns)
                {
                    if (column != null)
                    {
                        row.DropField(column);
                        if (firstIteration)
                        {
                            Columns.Remove(column);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to drop column!");
                    }
                }

                firstIteration = false;
            }
        }

        public Column GetColumn(string name)
        {
            Column column = Columns.Where(s => s.Name == name).FirstOrDefault();

            if (column == null)
            {
                Console.WriteLine("Failed to find column " + name + "!");
            }

            return column;
        }

        public List<Column> GetColumns(List<string> names)
        {
            List<Column> columns = new List<Column>();

            foreach (string name in names)
            {
                Column column = GetColumn(name);

                if (column != null)
                {
                    columns.Add(column);
                }
            }

            return columns;
        }

        public void CreateRow(Dictionary<string, string> fieldDictionary)
        {
            int rowNumber = Rows.Select(s => s.Number).LastOrDefault();
            Row row = new Row(rowNumber);

            foreach (KeyValuePair<string, string> fieldKeyValuePair in fieldDictionary)
            {
                Column column = GetColumn(fieldKeyValuePair.Key);
                Field field = new Field(fieldKeyValuePair.Value);
                row.CreateField(column, field);
            }

            Rows.Add(row);
        }

        public void DropRow(Row row)
        {
            if (row != null)
            {
                Rows.Remove(row);
            }
            else
            {
                Console.WriteLine("Failed to drop row!");
            }
        }

        public Row GetRow(int number)
        {
            Row row = Rows.Where(s => s.Number == number).FirstOrDefault();

            if (row == null)
            {
                Console.WriteLine("Failed to find row " + number + "!");
            }

            return row;
        }

        public Table Select(List<Column> columns)
        {
            Table table = Copy();
            List<Column> dropColumns = new List<Column>();
            
            foreach (Column column in table.Columns)
            {
                if (columns.Where(s => s.Name == column.Name).FirstOrDefault() == null)
                {
                    dropColumns.Add(column);
                }
            }

            table.DropColumns(dropColumns);

            return table;
        }

        public void ToOutput()
        {
            string line = "";
            Sql.Output.Add(ColumRowDivider());

            foreach (Column column in Columns)
            {
                int size = column.Size > column.Name.Length ? column.Size : column.Name.Length;
                line += "|" + column.Name;

                for (int i = column.Name.Length; i < size; i++)
                {
                    line += " ";
                }
            }

            line += "|";
            Sql.Output.Add(line);
            Sql.Output.Add(ColumRowDivider());

            foreach (Row row in Rows)
            {
                line = "";

                foreach (Column column in Columns)
                {
                    int size = column.Size > column.Name.Length ? column.Size : column.Name.Length;
                    line += "|" + row.GetField(column).Data;

                    for (int i = row.GetField(column).Data.Length; i < size; i++)
                    {
                        line += " ";
                    }
                }

                line += "|";
                Sql.Output.Add(line);
                Sql.Output.Add(ColumRowDivider());
            }
        }

        private string ColumRowDivider()
        {
            string line = "";

            foreach (Column column in Columns)
            {
                int size = column.Size > column.Name.Length ? column.Size : column.Name.Length;
                line += "+";

                for (int i = 0; i < size; i++)
                {
                    line += "-";
                }
            }

            line += "+";

            return line;
        }
    }
}
