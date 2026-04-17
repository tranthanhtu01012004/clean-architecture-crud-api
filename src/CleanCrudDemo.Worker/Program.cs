using CleanCrudDemo.Worker;
using CleanCrudDemo.Worker.Options;

var builder = Host.CreateApplicationBuilder(args);

// Bind RabbitMQ config
builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq"));

// Bind MongoDB config
builder.Services.Configure<MongoDbOptions>(
    builder.Configuration.GetSection("MongoDb"));

// Register Worker service
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();