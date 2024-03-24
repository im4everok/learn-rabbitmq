using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Producer
{
    public class ProducerService
    {
        public void Produce(object message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest",
                Port = 5672
            };

            // tcp connection
            using var connection = factory.CreateConnection();

            // virtual connection inside ^
            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "sample",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            string? serializedMessage = JsonSerializer.Serialize(message);

            var body = Encoding.UTF8.GetBytes(serializedMessage);

            channel.BasicPublish(
                exchange: "",
                routingKey: "sample",
                basicProperties: null,
                body: body);

            Console.WriteLine($"Sent a message: {message}");
        }
    }
}
