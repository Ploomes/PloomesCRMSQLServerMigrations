Configuration options = new();
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
UpgradeEngine generalUpgrader = DbUpUtilities.CreateUpgrader(options.Databases[selectedDatabase].ConnectionString, generalFilter);

Func<string, bool> procedureFilter = scriptName => scriptName.StartsWith("PloomesCRMSQLServerMigrations.Procedures.", StringComparison.OrdinalIgnoreCase);
UpgradeEngine procedureUpgrader = DbUpUtilities.CreateUpgraderWithNullJournal(options.Databases[selectedDatabase].ConnectionString, procedureFilter);

Func<string, bool> viewFilter = scriptName => scriptName.StartsWith("PloomesCRMSQLServerMigrations.Views.", StringComparison.OrdinalIgnoreCase);
UpgradeEngine viewUpgrader = DbUpUtilities.CreateUpgraderWithNullJournal(options.Databases[selectedDatabase].ConnectionString, viewFilter);

DbUpUtilities.ExecuteUpgrader(generalUpgrader);
DbUpUtilities.ExecuteUpgrader(procedureUpgrader);

string databaseName = options.Databases[selectedDatabase].Name;
if (databaseName != "QA")
{
    Func<string, bool> specificFilter = scriptName => scriptName.StartsWith("PloomesCRMSQLServerMigrations." + databaseName, StringComparison.OrdinalIgnoreCase);
    UpgradeEngine specificUpgrader = DbUpUtilities.CreateUpgrader(options.Databases[selectedDatabase].ConnectionString, specificFilter);
    DbUpUtilities.ExecuteUpgrader(specificUpgrader);
}

DbUpUtilities.ExecuteUpgrader(viewUpgrader);
