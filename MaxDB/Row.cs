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

        public Dictionary<Column, DataItem> DataItems { get; set; }

        public Row(int number)
        {
            Number = number;
            DataItems = new Dictionary<Column, DataItem>();
        }

        public void CreateDataItem(Column column, DataItem dataItem)
        {
            DataItems.Add(column, dataItem);
        }

        public void DropDataItem(Column column)
        {
            DataItems.Remove(column);
        }

        public DataItem GetDataItem(Column column)
        {
            return DataItems[column];
        }
    }
}
