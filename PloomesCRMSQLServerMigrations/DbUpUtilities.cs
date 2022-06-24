using DbUp.Engine;
using DbUp.Helpers;

namespace PloomesCRMSQLServerMigrations
{
    public class DbUpUtilities
    {
        public static UpgradeEngine CreateUpgrader(string connectionStrings, Func<string, bool> filter) => DeployChanges.To
                .SqlDatabase(connectionStrings)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), filter)
                .WithTransactionAlwaysRollback()
                .JournalToSqlTable("dbo", "SchemaVersions")
                .LogToConsole()
                .Build();

        public static UpgradeEngine CreateUpgraderWithNullJournal(string connectionStrings, Func<string, bool> filter) => DeployChanges.To
                .SqlDatabase(connectionStrings)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), filter)
                .WithTransactionAlwaysRollback()
                .JournalTo(new NullJournal())
                .LogToConsole()
                .Build();

        public static void ExecuteUpgrader(UpgradeEngine upgrader)
        { 
            Console.WriteLine("Starting upgrader...");

            //upgrader.GenerateUpgradeHtmlReport(<path>);
            
            // executed scripts
            /*var executedScripts = upgrader.GetExecutedScripts();
            if (executedScripts.Any())
                Console.WriteLine("Executed scripts:");
            Console.ForegroundColor = ConsoleColor.Blue;
            foreach (string executedScript in executedScripts)
                Console.WriteLine("- " + executedScript);
            Console.ResetColor();*/

            var scriptsToExecute = upgrader.GetScriptsToExecute();
            if (scriptsToExecute.Any())
                Console.WriteLine("Scripts to execute:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var scriptToExecute in scriptsToExecute)
                Console.WriteLine("- " + scriptToExecute.Name);
            Console.ResetColor();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(value: "Success!");
            Console.ResetColor();
        }
    }
}
