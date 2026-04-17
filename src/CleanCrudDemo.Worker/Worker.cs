using System.Text;
using System.Text.Json;
using CleanCrudDemo.Worker.Options;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Alias để tránh trùng tên với AuditLogDocument bên Infrastructure
using AuditLog = CleanCrudDemo.Worker.Models.AuditLogDocument;

namespace CleanCrudDemo.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly RabbitMqOptions _rabbitOptions;
    private readonly MongoDbOptions _mongoOptions;

    private IConnection? _connection;
    private IChannel? _channel;
    private IMongoCollection<AuditLog>? _collection;

    public Worker(
        ILogger<Worker> logger,
        IOptions<RabbitMqOptions> rabbitOptions,
        IOptions<MongoDbOptions> mongoOptions)
    {
        _logger = logger;
        _rabbitOptions = rabbitOptions.Value;
        _mongoOptions = mongoOptions.Value;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        // ==========================
        // 1. CONNECT MONGODB
        // ==========================
        var mongoClient = new MongoClient(_mongoOptions.ConnectionString);
        var database = mongoClient.GetDatabase(_mongoOptions.DatabaseName);
        _collection = database.GetCollection<AuditLog>(_mongoOptions.CollectionName);

        _logger.LogInformation(
            "Connected MongoDB: {Database}/{Collection}",
            _mongoOptions.DatabaseName,
            _mongoOptions.CollectionName);

        // ==========================
        // 2. CONNECT RABBITMQ
        // ==========================
        var factory = new ConnectionFactory
        {
            HostName = _rabbitOptions.HostName,
            UserName = _rabbitOptions.UserName,
            Password = _rabbitOptions.Password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        // ==========================
        // 3. DECLARE QUEUES
        // ==========================
        await _channel.QueueDeclareAsync(
            queue: _rabbitOptions.NewsQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: _rabbitOptions.MenuQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Connected RabbitMQ and declared queues: {NewsQueue}, {MenuQueue}",
            _rabbitOptions.NewsQueue,
            _rabbitOptions.MenuQueue);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null || _collection is null)
        {
            _logger.LogError("Worker initialization failed.");
            return;
        }

        // ==========================
        // CONSUMER: news-events
        // ==========================
        var newsConsumer = new AsyncEventingBasicConsumer(_channel);
        newsConsumer.ReceivedAsync += async (_, ea) =>
        {
            await ProcessMessageAsync(
                queueName: _rabbitOptions.NewsQueue,
                ea: ea,
                cancellationToken: stoppingToken);
        };

        await _channel.BasicConsumeAsync(
            queue: _rabbitOptions.NewsQueue,
            autoAck: false,
            consumer: newsConsumer,
            cancellationToken: stoppingToken);

        // ==========================
        // CONSUMER: menu-events
        // ==========================
        var menuConsumer = new AsyncEventingBasicConsumer(_channel);
        menuConsumer.ReceivedAsync += async (_, ea) =>
        {
            await ProcessMessageAsync(
                queueName: _rabbitOptions.MenuQueue,
                ea: ea,
                cancellationToken: stoppingToken);
        };

        await _channel.BasicConsumeAsync(
            queue: _rabbitOptions.MenuQueue,
            autoAck: false,
            consumer: menuConsumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("Worker is consuming RabbitMQ messages...");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessMessageAsync(
        string queueName,
        BasicDeliverEventArgs ea,
        CancellationToken cancellationToken)
    {
        if (_channel is null || _collection is null)
            return;

        try
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation(
                "Received message from queue {Queue}: {Message}",
                queueName,
                message);

            var auditDocument = BuildAuditDocument(queueName, message);

            await _collection.InsertOneAsync(
                auditDocument,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Saved structured log to MongoDB from queue {Queue}",
                queueName);

            await _channel.BasicAckAsync(
                deliveryTag: ea.DeliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error while processing message from queue {Queue}",
                queueName);

            if (_channel is not null)
            {
                await _channel.BasicNackAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: false,
                    cancellationToken: cancellationToken);
            }
        }
    }

    private AuditLog BuildAuditDocument(string queueName, string message)
    {
        using var jsonDoc = JsonDocument.Parse(message);
        var root = jsonDoc.RootElement;

        var eventName = GetStringProperty(root, "Event");
        var entityId = GetStringProperty(root, "Id");

        var action = ResolveAction(eventName);
        var entityName = ResolveEntityName(eventName);

        var payload = BsonDocument.Parse(message);

        return new AuditLog
        {
            QueueName = queueName,
            Event = eventName,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Payload = payload,
            ReceivedAtUtc = DateTime.UtcNow
        };
    }

    private static string GetStringProperty(JsonElement root, string propertyName)
    {
        if (root.TryGetProperty(propertyName, out var property) &&
            property.ValueKind == JsonValueKind.String)
        {
            return property.GetString() ?? string.Empty;
        }

        return string.Empty;
    }

    private static string ResolveAction(string eventName)
    {
        if (eventName.EndsWith("Created", StringComparison.OrdinalIgnoreCase))
            return "CREATE";

        if (eventName.EndsWith("Updated", StringComparison.OrdinalIgnoreCase))
            return "UPDATE";

        if (eventName.EndsWith("Deleted", StringComparison.OrdinalIgnoreCase))
            return "DELETE";

        return "UNKNOWN";
    }

    private static string ResolveEntityName(string eventName)
    {
        if (eventName.StartsWith("News", StringComparison.OrdinalIgnoreCase))
            return "News";

        if (eventName.StartsWith("Menu", StringComparison.OrdinalIgnoreCase))
            return "Menu";

        return "UnknownEntity";
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping worker...");

        if (_channel is not null)
            await _channel.CloseAsync(cancellationToken);

        if (_connection is not null)
            await _connection.CloseAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}