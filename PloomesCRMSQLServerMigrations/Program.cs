﻿Configuration options = new();
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, configuration) =>
    {
        configuration.Sources.Clear();
        IHostEnvironment env = hostingContext.HostingEnvironment;
        configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
            .Build()
            .Bind(options);
    })
    .Build();

Console.WriteLine("Select a database to deploy");
foreach(var item in options.Databases.Select((value, i) => new { i, value }))
{
    Console.WriteLine(item.i + " - " + item.value.Name);
}

int selectedDatabase = Convert.ToInt32(Console.ReadLine());

Func<string, bool> generalFilter = scriptName => scriptName.StartsWith("PloomesCRMSQLServerMigrations.Scripts.", StringComparison.OrdinalIgnoreCase);
var generalUpgrader = DbUpUtilities.CreateUpgrader(options.Databases[selectedDatabase].ConnectionString, generalFilter);

Func<string, bool> procedureFilter = scriptName => scriptName.StartsWith("PloomesCRMSQLServerMigrations.Procedures.", StringComparison.OrdinalIgnoreCase);
var procedureUpgrader = DbUpUtilities.CreateUpgraderWithNullJournal(options.Databases[selectedDatabase].ConnectionString, procedureFilter);

Func<string, bool> specificFilter = scriptName => scriptName.StartsWith("PloomesCRMSQLServerMigrations." + options.Databases[selectedDatabase].Name, StringComparison.OrdinalIgnoreCase);
var specificUpgrader = DbUpUtilities.CreateUpgrader(options.Databases[selectedDatabase].ConnectionString, specificFilter);

Func<string, bool> viewFilter = scriptName => scriptName.StartsWith("PloomesCRMSQLServerMigrations.Views.", StringComparison.OrdinalIgnoreCase);
var viewUpgrader = DbUpUtilities.CreateUpgraderWithNullJournal(options.Databases[selectedDatabase].ConnectionString, viewFilter);

DbUpUtilities.ExecuteUpgrader(generalUpgrader);
DbUpUtilities.ExecuteUpgrader(procedureUpgrader);
DbUpUtilities.ExecuteUpgrader(specificUpgrader);
DbUpUtilities.ExecuteUpgrader(viewUpgrader);
