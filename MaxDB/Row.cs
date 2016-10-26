using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxDB
{
    public class Row
    {
        public int Number { get; set; }

        public Dictionary<Column, Field> Fields { get; set; }

        public Row(int number)
        {
            Number = number;
            Fields = new Dictionary<Column, Field>();
        }

        public void CreateField(Column column, Field field)
        {
            Fields.Add(column, field);
        }

        public void DropField(Column column)
        {
            Fields.Remove(column);
        }

        public Field GetField(Column column)
        {
            return Fields[column];
        }
    }
}
