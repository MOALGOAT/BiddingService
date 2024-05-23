using RabbitMQ.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class BidSender : BackgroundService
{
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<BidSender> _logger;

    public BidSender(IServiceScopeFactory serviceScopeFactory, ILogger<BidSender> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        var factory = new ConnectionFactory { HostName = "localhost" }; // Use the hostname defined in Docker Compose
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SendMessageAsync("Hello World!");
                await Task.Delay(5000, stoppingToken); // Send a message every 5 seconds
            }
        }, stoppingToken);

        return Task.CompletedTask;
    }

    private async Task SendMessageAsync(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "",
                              routingKey: "hello",
                              basicProperties: null,
                              body: body);

        _logger.LogInformation($" [x] Sent {message}");

        using var scope = _serviceScopeFactory.CreateScope();
        var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();
        await auctionService.SomeMethodAsync();
    }

    public override void Dispose()
    {
        _channel.Close();
        _channel.Dispose();
        base.Dispose();
    }
}

public interface IAuctionService
{
    Task SomeMethodAsync();
}
