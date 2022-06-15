using DbUp;
using Microsoft.Extensions.Configuration;
using System.Reflection;

var configuration = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile($"appsettings.json");

var config = configuration.Build();
var connectionString = config["ConnectionString"];
var shard = config["Shard"];

var xd = Assembly.GetExecutingAssembly();

var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), (string s) => !s.StartsWith("PloomesCRMSQLServerMigrations.Scripts.Shard") 
                                                                    || (!string.IsNullOrEmpty(shard) && s.StartsWith("PloomesCRMSQLServerMigrations.Scripts." + shard)))
    .LogToConsole()
    .Build();

var executedScripts = upgrader.GetExecutedScripts();
if (executedScripts.Any())
    Console.WriteLine("Executed scripts:");
Console.ForegroundColor = ConsoleColor.Blue;
foreach (string executedScript in executedScripts)
    Console.WriteLine("- " + executedScript);
Console.ResetColor();

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
