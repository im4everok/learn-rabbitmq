using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer
{
    public class ConsumerService
    {
        public string Consume()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq",
            UserName = "guest", Password = "guest", Port = 5672 };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "sample",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            string result = "";

            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                result = message;
            };

            channel.BasicConsume(
                queue: "sample",
                autoAck: true,
                consumer: consumer
                );

            return result;
        }
    }
}
