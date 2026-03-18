using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("👨‍🍳 Chef (Worker) is getting ready...");

// 1. Connect to the same RabbitMQ server
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

const string queueName = "game_updates";

// 2. Ensure the window exists (just in case the Chef starts working before the Waiter)
await channel.QueueDeclareAsync(queue: queueName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

Console.WriteLine($"👨‍🍳 Chef is staring at the '{queueName}' window...");

// 3. Create the Consumer (The Chef's eyes and hands)
var consumer = new AsyncEventingBasicConsumer(channel);

// 4. Tell the Chef exactly what to do when a ticket appears!
consumer.ReceivedAsync += async (model, ea) =>
{
    // A. Read the raw bytes and turn them back into a string
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    
    Console.WriteLine($"\n[x] 🎟️ TICKET RECEIVED!");
    Console.WriteLine($"[x] Order details: {message}");
    
    // B. Simulate the "heavy lifting" (like sending 10,000 emails or processing stats)
    Console.WriteLine("[x] Chef is doing heavy background work... ⏳");
    await Task.Delay(3000); // Wait for 3 seconds to simulate hard work
    
    Console.WriteLine("[x] Order is complete! 🍲");

    // C. Tell RabbitMQ to throw the ticket in the trash so it doesn't get processed twice
    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
};

// 5. Tell the system to start listening!
// Notice "autoAck: false" - this means RabbitMQ won't delete the message until the Chef explicitly says it's done (Step C).
await channel.BasicConsumeAsync(queue: queueName,
                                autoAck: false,
                                consumer: consumer);

// 6. Keep the console application running forever
Console.WriteLine(" Press [enter] to close the kitchen.");
Console.ReadLine();