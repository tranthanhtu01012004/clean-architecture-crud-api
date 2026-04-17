using System.Text;
using System.Text.Json;
using CleanCrudDemo.Application.Abstractions;
using RabbitMQ.Client;

namespace CleanCrudDemo.Infrastructure.Messaging;

public class RabbitMqPublisher : IEventPublisher
{
    private readonly ConnectionFactory _factory; // RabbitMQ Client: factory tạo kết nối RabbitMQ.

    public RabbitMqPublisher(string hostName, string userName, string password)
    {
        _factory = new ConnectionFactory // RabbitMQ Client: cấu hình connection.
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };
    }

    public async Task PublishAsync(string queueName, object message, CancellationToken cancellationToken)
    {
        await using var connection = await _factory.CreateConnectionAsync(cancellationToken); // RabbitMQ Client: mở connection async.
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken); // RabbitMQ Client: tạo channel async.

        await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken); // RabbitMQ Client: đảm bảo queue tồn tại.

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)); // System.Text.Json + Encoding: serialize message thành byte[].
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body, cancellationToken: cancellationToken); // RabbitMQ Client: publish message vào queue.
    }
}
