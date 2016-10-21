using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxDB
{
    public static class Sql
    {
        public static Database CurrentDatabase { get; set; }

        public static List<string> Digests { get; set; }

        private static string Digest
        {
            get
            {
                string currentDigest = Digests.Select(s => s).LastOrDefault();
                return currentDigest;
            }
            set
            {
                string currentDigest = Digests.Select(s => s).LastOrDefault();
                Digests.Remove(currentDigest);
                Digests.Add(value);
            }
        }

        static Sql()
        {
            CurrentDatabase = null;
            Digests = new List<string>();
            Digest = null;
        }

        public static void Parse(string sqlString)
        {
            List<string> statements = GetStatements(sqlString.Trim());

            foreach (string statement in statements)
            {
                Execute(statement.Trim());
            }
        }

        internal static bool IsStatement(string input)
        {
            bool isStatement = false;

            if (input.Contains(";"))
            {
                isStatement = true;
            }

            return isStatement;
        }

        private static List<string> GetStatements(string sqlString)
        {
            char[] delimeters = new char[] { ';' };
            string[] statements = sqlString.Split(delimeters);

            return statements.ToList();
        }

        private static string GetNextDigestSegment(char end)
        {
            char[] delimeters = new char[] { end };
            string[] segments = Digest.Split(delimeters, 2);

            if (segments.Length > 1)
            {
                Digest = segments[1].Trim();
            }
            else
            {
                Digest = null;
            }

            return segments[0].Trim();
        }

        private static void Execute(string statement)
        {
            Digest = statement;
            string command = GetNextDigestSegment(' ');

            switch (command)
            {
                case "create":
                    Create();
                    break;

                case "drop":
                    Drop();
                    break;

                case "use":
                    Use();
                    break;

                case "show":
                    Show();
                    break;
            }
        }

        private static void Show()
        {
            string context = GetNextDigestSegment(' ');

            switch (context)
            {
                case "databases":
                    DatabaseCollection.ShowDatabases();
                    break;

                case "tables":
                    if (CurrentDatabase != null)
                    {
                        CurrentDatabase.ShowTables();
                    }
                    else
                    {
                        Console.WriteLine("Please select a database first.");
                    }
                    break;
            }
        }

        private static void Use()
        {
            string databaseName = GetNextDigestSegment(' ');

            CurrentDatabase = DatabaseCollection.GetDatabase(databaseName);
        }

        private static void Create()
        {
            string context = GetNextDigestSegment(' ');

            switch (context)
            {
                case "database":
                    DatabaseCollection.CreateDatabase(GetNextDigestSegment(' '));
                    break;

                case "table":
                    if (CurrentDatabase != null)
                    {
                        string tableName = GetNextDigestSegment(' ');
                        CurrentDatabase.CreateTable(tableName);
                        Table table = CurrentDatabase.GetTable(tableName);

                        //string block = null;

                        if (BeginBlock())
                        {
                            SwitchDigestBlock();
                            //block = GetBlock();

                            while (Digest.Contains(','))
                            {
                                string columnString = GetNextDigestSegment(',');
                            }
                        }
                        else
                        {
                            Console.WriteLine("Syntax Error!");
                            CurrentDatabase.DropTable(tableName);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please select a database first.");
                    }
                    break;
            }
        }

        private static void SwitchDigestBlock()
        {
            string block = "";
            int openParenthesesCount = 0;
            int blockEndCharIndex = 0;

            for (int i = 0; i < Digest.Length; i++)
            {
                if (Digest[i] == '(')
                {
                    openParenthesesCount++;
                }
                else if (Digest[i] == ')')
                {
                    openParenthesesCount--;
                }

                if (openParenthesesCount == 0)
                {
                    blockEndCharIndex = i;
                    break;
                }
            }

            block = Digest.Substring(1, blockEndCharIndex - 1).Trim();

            if (Digest.Length > blockEndCharIndex + 1)
            {
                Digest = Digest.Substring(blockEndCharIndex + 1).Trim();
            }
            else
            {
                Digest = null;
            }

            Digests.Add(block);
        }

        private static string GetBlock()
        {
            string block = "";
            int openParenthesesCount = 0;
            int blockEndCharIndex = 0;

            for (int i = 0; i < Digest.Length; i++)
            {
                if (Digest[i] == '(')
                {
                    openParenthesesCount++;
                }
                else if (Digest[i] == ')')
                {
                    openParenthesesCount--;
                }

                if (openParenthesesCount == 0)
                {
                    blockEndCharIndex = i;
                    break;
                }
            }

            block = Digest.Substring(1, blockEndCharIndex - 1).Trim();

            if (Digest.Length > blockEndCharIndex + 1)
            {
                Digest = Digest.Substring(blockEndCharIndex + 1).Trim();
            }
            else
            {
                Digest = null;
            }

            return block;
        }

        private static bool BeginBlock()
        {
            bool beginBlock = false;
            char firstChar = Digest[0];

            if (firstChar == '(')
            {
                beginBlock = true;
            }

            return beginBlock;
        }

        private static void Drop()
        {
            string context = GetNextDigestSegment(' ');

            switch (context)
            {
                case "database":
                    DatabaseCollection.DropDatabase(GetNextDigestSegment(' '));
                    break;

                case "table":
                    if (CurrentDatabase != null)
                    {
                        CurrentDatabase.DropTable(GetNextDigestSegment(' '));
                    }
                    else
                    {
                        Console.WriteLine("Please select a database first.");
                    }
                    break;
            }
        }
    }
}
