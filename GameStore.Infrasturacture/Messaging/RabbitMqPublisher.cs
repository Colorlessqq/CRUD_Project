using System.Text;
using System.Text.Json;
using GameStore.Application.Interfaces;
using RabbitMQ.Client; // This must be at the top

namespace GameStore.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessagePublisher
{
    // Change: Added 'async Task' to the method signature
    public async Task PublishMessageAsync<T>(T message, string queueName)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        // New Version 7 Method: CreateConnectionAsync()
        using var connection = await factory.CreateConnectionAsync();
        
        // New Version 7 Method: CreateChannelAsync()
        using var channel = await connection.CreateChannelAsync();

        // Ensure the queue exists
        await channel.QueueDeclareAsync(queue: queueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        // New Version 7 Method: BasicPublishAsync()
        await channel.BasicPublishAsync(exchange: string.Empty,
                                        routingKey: queueName,
                                        body: body);

        Console.WriteLine($"[RabbitMQ] 🐇 Ticket dropped in window: {queueName}");
    }
}