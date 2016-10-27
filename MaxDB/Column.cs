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

        public int MaxFieldSize { get; set; }

        public Column(string name, string dataType, int size)
        {
            Name = name;
            DataType = dataType;
            Size = size;
            MaxFieldSize = 0;
        }

        public int GetSizeToOutput()
        {
            int size = MaxFieldSize < Size ? MaxFieldSize : Size;
            size = Name.Length > size ? Name.Length : size;

            return size;
        }
    }
}
