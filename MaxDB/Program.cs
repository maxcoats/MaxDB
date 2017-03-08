using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Count() > 0)
            {
                if (File.Exists(args[0]))
                {
                    using (StreamReader streamReader = new StreamReader(args[0]))
                    {
                        string fileInput = "";

                        while (!streamReader.EndOfStream)
                        {
                            fileInput += " " + streamReader.ReadLine().Trim();

                            if (DatabaseEngine.IsStatement(fileInput))
                            {
                                DatabaseEngine.Parse(fileInput);
                                fileInput = "";
                            }
                        }
                    }
                }
            }

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
