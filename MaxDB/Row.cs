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

        public Dictionary<string, DataItem> DataItems { get; set; }

        public Row(int number)
        {
            Number = number;
            DataItems = new Dictionary<string, DataItem>();
        }

        public void CreateDataItem(Column column, DataItem dataItem)
        {
            DataItems.Add(column.Name, dataItem);
        }

        public void DropDataItem(Column column)
        {
            DataItems.Remove(column.Name);
        }

        public DataItem GetDataItem(Column column)
        {
            return DataItems[column.Name];
        }
    }
}
