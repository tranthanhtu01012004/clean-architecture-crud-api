namespace CleanCrudDemo.Worker.Options;

public class MongoDbOptions
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "CleanCrudDemoAuditDb";
    public string CollectionName { get; set; } = "audit_logs";
}