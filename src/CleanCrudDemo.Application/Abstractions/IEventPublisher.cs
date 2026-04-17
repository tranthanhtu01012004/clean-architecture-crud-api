namespace CleanCrudDemo.Application.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync(string queueName, object message, CancellationToken cancellationToken); // RabbitMQ: publish message vào queue.
}
