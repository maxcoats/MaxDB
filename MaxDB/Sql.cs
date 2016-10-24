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
                
                if (currentDigest != null)
                {
                    Digests.Remove(currentDigest);
                }

                if (value != null && value != "")
                {
                    Digests.Add(value);
                }
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

            if (input != "")
            {
                if (input[input.Length - 1] == ';')
                {
                    isStatement = true;
                }
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

        private static string GetEndOfDigest()
        {
            string digest = Digest;
            Digest = null;

            return digest;
        }

        private static void Execute(string statement)
        {
            Digest = statement;

            if (Digest != null)
            {
                string command = GetNextDigestSegment(' ');
                Digest = Digest.Replace("(", " (");

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

                    case "insert":
                        if (GetNextDigestSegment(' ') == "into")
                        {
                            Insert();
                        }
                        else
                        {
                            Console.WriteLine("Syntax Error!");
                        }
                        break;
                }
            }
        }

        private static void Insert()
        {
            string tableName = GetNextDigestSegment(' ');
            Table table = CurrentDatabase.GetTable(tableName);
            string context = null;

            if (BeginScope('('))
            {
                context = "";
            }
            else
            {
                context = GetNextDigestSegment(' ');
            }

            switch (context)
            {
                case "values":
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

                        if (ContainsScope('(', ')') && BeginScope('('))
                        {
                            SwitchDigestScope('(', ')');
                            Digest = Digest + ",";
                            int columnsDigestIndex = Digests.IndexOf(Digest);

                            while (Digests.Count > columnsDigestIndex)
                            {
                                while (columnsDigestIndex < Digests.IndexOf(Digest))
                                {
                                    Digest = null;
                                }

                                if (Digests[columnsDigestIndex].Contains(','))
                                {
                                    string columnString = GetNextDigestSegment(',');
                                    Digests.Add(columnString);
                                    string columnName = GetNextDigestSegment(' ');
                                    string columnDataType = GetNextDigestSegment(' ');
                                    int columnSize = 0;

                                    if (ContainsScope('(', ')') && BeginScope('('))
                                    {
                                        SwitchDigestScope('(', ')');
                                        columnSize = int.Parse(GetEndOfDigest());

                                        table.CreateColumn(columnName, columnDataType, columnSize);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Syntax Error!");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Syntax Error!");
                                }
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

        private static void SwitchDigestScope(char scopeStartChar, char scopeEndChar)
        {
            string scope = "";
            int scopeStartCharCount = 0;
            int scopeStartCharIndex = 0;
            int scopeEndCharIndex = 0;

            for (int i = 0; i < Digest.Length; i++)
            {
                if (Digest[i] == scopeStartChar)
                {
                    if (scopeStartCharCount == 0)
                    {
                        scopeStartCharIndex = i;
                    }

                    scopeStartCharCount++;
                }
                else if (Digest[i] == scopeEndChar)
                {
                    scopeStartCharCount--;
                }

                if (scopeStartCharCount == 0)
                {
                    scopeEndCharIndex = i;
                    break;
                }
            }

            scope = Digest.Substring(scopeStartCharIndex + 1, scopeEndCharIndex - 1).Trim();

            if (Digest.Length > scopeEndCharIndex + 1)
            {
                Digest = Digest.Substring(scopeEndCharIndex + 1).Trim();
            }
            else
            {
                Digest = null;
            }

            Digests.Add(scope);
        }

        private static bool ContainsScope(char scopeStartChar, char scopeEndChar)
        {
            bool containsScope = false;
            
            if (scopeStartChar != scopeEndChar)
            {
                if (Digest.Contains(scopeStartChar.ToString()) && Digest.Contains(scopeEndChar.ToString()))
                {
                    containsScope = true;
                }
                else if (Digest.Substring(Digest.IndexOf(scopeStartChar) + 1).Contains(scopeEndChar))
                {
                    containsScope = true;
                }
            }

            return containsScope;
        }

        private static bool BeginScope(char scopeStartChar)
        {
            bool beginScope = false;

            if (Digest[0] == scopeStartChar)
            {
                beginScope = true;
            }

            return beginScope;
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
