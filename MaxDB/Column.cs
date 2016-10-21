using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxDB
{
    public class Column
    {
        public string Name { get; set; }

        public string DataType { get; set; }

        public int Size { get; set; }

        public Column(string name, string dataType, int size)
        {
            Name = name;
            DataType = dataType;
            Size = size;
        }
    }
}
