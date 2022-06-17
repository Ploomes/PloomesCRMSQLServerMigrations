namespace PloomesCRMSQLServerMigrations
{
    public class Configuration
    {
        public List<Database> Databases { get; set; }
    }

    public class Database
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}
