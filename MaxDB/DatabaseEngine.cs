using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaxDB.Utilities;

namespace MaxDB
{
    public static class DatabaseEngine
    {
        public static DatabaseCollection DatabaseCollection { get; set; }

        public static Database CurrentDatabase { get; set; }

        public static List<string> DisplayBuffer { get; set; }

        public static List<string> Digests { get; set; }

        private static string Digest
        {
            get
            {
                string currentDigest = Digests.LastOrDefault();

                if (currentDigest == null)
                {
                    currentDigest = "";
                }

                return currentDigest;
            }
            set
            {
                string currentDigest = Digests.LastOrDefault();
                
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

        static DatabaseEngine()
        {
            DatabaseCollection = new DatabaseCollection();
            CurrentDatabase = null;
            DisplayBuffer = new List<string>();
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
                Digest = Digest.Replace("(", " (");
                Digest = Digest.Replace(",", ", ");
                string command = GetNextDigestSegment(' ');

                switch (command.ToLower())
                {
                    case "create":
                        Create();
                        StorageUtility.WriteDatabaseCollectionToDisk(DatabaseCollection.Databases);
                        break;

                    case "drop":
                        Drop();
                        StorageUtility.WriteDatabaseCollectionToDisk(DatabaseCollection.Databases);
                        break;

                    case "use":
                        Use();
                        break;

                    case "show":
                        Show();
                        break;

                    case "insert":
                        if (CurrentDatabase != null)
                        {
                            if (GetNextDigestSegment(' ').ToLower() == "into")
                            {
                                Insert();
                                StorageUtility.WriteDatabaseCollectionToDisk(DatabaseCollection.Databases);
                            }
                            else
                            {
                                Console.WriteLine("Syntax Error!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Please select a database first.");
                        }
                        break;

                    case "select":
                        if (CurrentDatabase != null)
                        {
                            Select();
                        }
                        else
                        {
                            Console.WriteLine("Please select a database first.");
                        }
                        break;
                }
            }
        }

        private static void Select()
        {
            List<string> columnNames = new List<string>();
            string nextDigestSegment = "";

            while ((nextDigestSegment = GetNextDigestSegment(' ').Trim(',')).ToLower() != "from")
            {
                columnNames.Add(nextDigestSegment);
            }

            if (columnNames.Count > 0)
            {
                if (nextDigestSegment.ToLower() == "from")
                {
                    string tableName = GetNextDigestSegment(' ');
                    Table table = CurrentDatabase.GetTable(tableName);

                    if (table != null)
                    {
                        if (columnNames.FirstOrDefault() == "*")
                        {
                            table.ToDisplayBuffer();
                            DisplayBufferToConsole();
                        }
                        else
                        {
                            Table tempTaple = table.Select(table.GetColumns(columnNames));
                            tempTaple.ToDisplayBuffer();
                            DisplayBufferToConsole();
                        }
                    }
                }
            }
        }

        private static void Insert()
        {
            string tableName = GetNextDigestSegment(' ');
            Table table = CurrentDatabase.GetTable(tableName);

            if (table != null)
            {
                string context = null;

                if (BeginScope('('))
                {
                    context = "";
                }
                else
                {
                    context = GetNextDigestSegment(' ');
                }

                switch (context.ToLower())
                {
                    case "values":
                        InsertValues(table, table.Columns.Select(s => s.Name).ToList());
                        break;
                }
            }
            else
            {
                Console.WriteLine("No table named " + tableName + " exists!");
            }
        }

        private static void InsertValues(Table table, List<string> columnNames)
        {
            if (ContainsScope('(', ')') && BeginScope('('))
            {
                SwitchDigestScope('(', ')');
                Digest += ",";
                int dataItemsDigestIndex = Digests.IndexOf(Digest);
                List<string> dataItemStrings = new List<string>();

                while (Digests.Count > dataItemsDigestIndex)
                {
                    if (Digest.Contains(','))
                    {
                        string dataItemString = GetNextDigestSegment(',');
                        dataItemStrings.Add(dataItemString);
                    }
                    else
                    {
                        Console.WriteLine("Syntax Error!");
                    }
                }

                Dictionary<string, string> dataItemDictionary = new Dictionary<string, string>();

                foreach (string columnName in columnNames)
                {
                    string dataItemString = dataItemStrings.FirstOrDefault();
                    dataItemDictionary.Add(columnName, dataItemString);
                    dataItemStrings.Remove(dataItemString);
                }

                table.CreateRow(dataItemDictionary);
            }
            else
            {
                Console.WriteLine("Syntax Error!");
            }

        }

        private static void Show()
        {
            string context = GetNextDigestSegment(' ');

            switch (context.ToLower())
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

            if (CurrentDatabase == null)
            {
                Console.WriteLine("No database named " + databaseName + " exists!");
            }
        }

        private static void Create()
        {
            string context = GetNextDigestSegment(' ');

            switch (context.ToLower())
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
                            Digest += ",";
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
                                    string columnDataType = GetNextDigestSegment(' ').ToLower();
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

            if (scopeStartChar != scopeEndChar)
            {
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
            }
            else
            {
                scopeStartCharIndex = Digest.IndexOf('\'');
                scopeEndCharIndex = Digest.Substring(scopeStartCharIndex + 1).IndexOf('\'') + scopeStartCharIndex + 1;
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
            }
            else if (Digest.Substring(Digest.IndexOf(scopeStartChar) + 1).Contains(scopeEndChar))
            {
                containsScope = true;
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

            switch (context.ToLower())
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

        public static void DisplayBufferToConsole()
        {
            foreach(string line in DisplayBuffer)
            {
                Console.WriteLine(line);
            }

            DisplayBuffer.Clear();
        }
    }
}
