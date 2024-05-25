using BiddingServiceAPI.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace BiddingServiceAPI.Service
{

    public interface IBiddingInterface
    {
        public string AddBid(Bid bid);
        public void RefreshAuctions();
    }

    public class BiddingService : IBiddingInterface
    {
            private readonly IModel _channel;
        private readonly ILogger<BiddingService> _logger;

        // lave en private Dictionary<string(auctionId),auction> 

        public BiddingService(ILogger<BiddingService> logger, IConfiguration configuration)
        {
            _logger = logger;
            var factory = new ConnectionFactory { HostName = Environment.GetEnvironmentVariable("QueueHostName") }; // Use the hostname defined in Docker Compose
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "bid_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public string AddBid(Bid bid)
        {

            _logger.LogInformation(bid.ToString());
            // Check if bid is valid
            //if (Bidisvalid)
            if (true)
            {
                var body = JsonSerializer.Serialize<Bid>(bid);
                _channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: "bids",
                    mandatory: false, // Add the missing 'mandatory' argument
                    basicProperties: null,
                    body: Encoding.UTF8.GetBytes(body)
                );

                // Log the bid object directly
                _logger.LogInformation("Bid received: {@Bid}", bid);

                return "bid accepted, xxx klokkeslæt";
            }
            else
            {
                _logger.LogWarning("Bid is not valid");
                return "bid not accepted, xxx";
            }
        }


        public void RefreshAuctions()
        {
            //opdater dictionary med auctions fra http request (dagens auktioner)
            //linq til "privat"(mindre attributer)
            //graphQL
            //kaldes dagligt 
            // ved startup
        }

    }
}
