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

        public void CreateColumn(string name, string dataItemType, int size)
        {
            if (!IsColumn(name))
            {
                Column column = new Column(name, dataItemType, size);
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
                    row.DropDataItem(column);
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
                        row.DropDataItem(column);
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

        public void CreateRow(Dictionary<string, string> dataItemDictionary)
        {
            Row row = new Row();
            bool addRow = true;

            foreach (KeyValuePair<string, string> dataItemKeyValuePair in dataItemDictionary)
            {
                string value = dataItemKeyValuePair.Value;
                Column column = GetColumn(dataItemKeyValuePair.Key);

                if (column.TypeCheck(value))
                {
                    if (column.DataItemType == "varchar")
                    {
                        value = value.Trim('\'');
                    }

                    if (column.Size < value.Length)
                    {
                        value = value.Substring(0, column.Size);
                    }

                    if (column.MaxDataItemSize < value.Length)
                    {
                        column.MaxDataItemSize = value.Length;
                    }

                    DataItem dataItem = new DataItem(value);
                    row.CreateDataItem(column, dataItem);
                }
                else
                {
                    addRow = false;
                }
            }

            if (addRow)
            {
                Rows.Add(row);
            }
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

        public Table Select(List<Column> columns)
        {
            return Copy(columns);
        }

        public Table Copy()
        {
            Table table = new Table(Name);

            foreach (Column column in Columns)
            {
                table.CreateColumn(column.Name, column.DataItemType, column.Size);
            }

            foreach (Row row in Rows)
            {
                Dictionary<string, string> dataItemDictionary = new Dictionary<string, string>();

                foreach (Column column in Columns)
                {
                    dataItemDictionary.Add(column.Name, row.GetDataItem(column).Value);
                }

                table.CreateRow(dataItemDictionary);
            }

            return table;
        }

        public Table Copy(List<Column> columns)
        {
            Table table = new Table(Name);

            foreach (Column column in columns)
            {
                Column copyColumn = GetColumn(column.Name);

                if (copyColumn != null)
                {
                    table.CreateColumn(copyColumn.Name, copyColumn.DataItemType, copyColumn.Size);
                }
            }

            foreach (Row row in Rows)
            {
                Dictionary<string, string> dataItemDictionary = new Dictionary<string, string>();

                foreach (Column column in columns)
                {
                    Column copyColumn = GetColumn(column.Name);

                    if (copyColumn != null)
                    {
                        dataItemDictionary.Add(copyColumn.Name, row.GetDataItem(copyColumn).Value);
                    }
                }

                if (dataItemDictionary.Count > 0)
                {
                    table.CreateRow(dataItemDictionary);
                }
            }

            return table;
        }

        public void ToDisplayBuffer()
        {
            string line = "";

            if (Columns.Count > 0)
            {
                DatabaseEngine.DisplayBuffer.Add(ColumRowDivider());
            }

            foreach (Column column in Columns)
            {
                int size = column.GetSizeToOutput();
                line += "|" + column.Name;

                for (int i = column.Name.Length; i < size; i++)
                {
                    line += " ";
                }
            }

            if (Columns.Count > 0)
            {
                line += "|";
                DatabaseEngine.DisplayBuffer.Add(line);
                DatabaseEngine.DisplayBuffer.Add(ColumRowDivider());
            }

            foreach (Row row in Rows)
            {
                line = "";

                foreach (Column column in Columns)
                {
                    int size = column.GetSizeToOutput();
                    line += "|" + row.GetDataItem(column).Value;

                    for (int i = row.GetDataItem(column).Value.Length; i < size; i++)
                    {
                        line += " ";
                    }
                }

                line += "|";
                DatabaseEngine.DisplayBuffer.Add(line);
                DatabaseEngine.DisplayBuffer.Add(ColumRowDivider());
            }
        }

        private string ColumRowDivider()
        {
            string line = "";

            foreach (Column column in Columns)
            {
                int size = column.GetSizeToOutput();
                line += "+";

                for (int i = 0; i < size; i++)
                {
                    line += "-";
                }
            }

            if (Columns.Count > 0)
            {
                line += "+";
            }

            return line;
        }
    }
}
