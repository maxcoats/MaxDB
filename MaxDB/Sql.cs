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

        static Sql()
        {
            CurrentDatabase = null;
        }

        public static void Parse(string sqlString)
        {
            List<string> statements = GetStatements(sqlString);

            foreach (string statement in statements)
            {
                Execute(statement);
            }
        }

        private static List<string> GetStatements(string sqlString)
        {
            char[] delimeters = new char[] { ';' };
            string[] statements = sqlString.Split(delimeters);

            return statements.ToList();
        }

        private static void Execute(string statement)
        {
            List<string> statementSegments = GetStatementSegments(statement);
            string command = statementSegments.Select(s => s).FirstOrDefault();
            statementSegments.Remove(command);

            switch (command.Trim())
            {
                case "create":
                    Create(statementSegments);
                    break;

                case "drop":
                    Drop(statementSegments);
                    break;

                case "use":
                    Use(statementSegments);
                    break;
            }
        }

        private static void Use(List<string> statementSegments)
        {
            string databaseName = GetNextStatementSegment(statementSegments);

            CurrentDatabase = DatabaseCollection.GetDatabase(databaseName);
        }

        private static List<string> GetStatementSegments(string statement)
        {
            char[] delimeters = new char[] { ' ' };
            string[] statementSegments = statement.Split(delimeters);

            return statementSegments.ToList();
        }

        private static void Create(List<string> statementSegments)
        {
            string context = GetNextStatementSegment(statementSegments);

            switch (context.Trim())
            {
                case "database":
                    DatabaseCollection.CreateDatabase(GetNextStatementSegment(statementSegments));
                    break;

                case "table":
                    if (CurrentDatabase != null)
                    {
                        CurrentDatabase.CreateTable(GetNextStatementSegment(statementSegments));
                    }
                    else
                    {
                        Console.WriteLine("Please select a database first.");
                    }
                    break;
            }
        }

        private static void Drop(List<string> statementSegments)
        {
            string context = statementSegments.Select(s => s).FirstOrDefault();
            statementSegments.Remove(context);

            switch (context.Trim())
            {
                case "database":
                    DatabaseCollection.DropDatabase(GetNextStatementSegment(statementSegments));
                    break;

                case "table":
                    if (CurrentDatabase != null)
                    {
                        CurrentDatabase.DropTable(GetNextStatementSegment(statementSegments));
                    }
                    else
                    {
                        Console.WriteLine("Please select a database first.");
                    }
                    break;
            }
        }

        private static string GetNextStatementSegment(List<string> statementSegments)
        {
            string segment = statementSegments.Select(s => s).FirstOrDefault();
            statementSegments.Remove(segment);

            return segment.Trim();
        }
    }
}
