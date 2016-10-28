using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                string input = "";

                while (!DatabaseEngine.IsStatement(input))
                {
                    input += " " + Console.ReadLine().Trim();
                }

                DatabaseEngine.Parse(input);
            }
        }
    }
}
