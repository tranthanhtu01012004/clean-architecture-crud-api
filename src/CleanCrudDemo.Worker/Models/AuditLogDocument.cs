using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CleanCrudDemo.Worker.Models;

public class AuditLogDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string QueueName { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public BsonDocument Payload { get; set; } = new();
    public DateTime ReceivedAtUtc { get; set; }
}