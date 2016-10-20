using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxDB
{
    public class Row
    {
        public int RowNumber { get; set; }

        public Dictionary<Column, Field> RowField { get; set; }
    }
}
