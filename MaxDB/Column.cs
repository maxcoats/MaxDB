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

        public string DataItemType { get; set; }

        public int Size { get; set; }

        public int MaxDataItemSize { get; set; }

        public Column(string name, string dataItemType, int size)
        {
            Name = name;
            DataItemType = dataItemType;
            Size = size;
            MaxDataItemSize = 0;
        }

        public int GetSizeToOutput()
        {
            int size = MaxDataItemSize < Size ? MaxDataItemSize : Size;
            size = Name.Length > size ? Name.Length : size;

            return size;
        }

        public bool TypeCheck(string value)
        {
            return true;
        }
    }
}
