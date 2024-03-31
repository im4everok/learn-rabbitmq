using Consumer;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ConsumerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var factory = new ConnectionFactory()
{
    HostName = "rabbitmq",
    UserName = "guest",
    Password = "guest",
    Port = 5672
};

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.QueueDeclare(
    queue: "sample",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new EventingBasicConsumer(channel);

//string result = "";

consumer.Received += (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    //result = message;
    Console.WriteLine($"Consumed: {message}");
};

channel.BasicConsume(
    queue: "sample",
    autoAck: true,
    consumer: consumer
    );

//app.MapPost("consume-message", async (ConsumerService consumerService) =>
//{
//    var result = consumerService.Consume();

//    return result;
//});

app.Run();
