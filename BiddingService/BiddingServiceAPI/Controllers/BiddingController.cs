using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BiddingServiceAPI.Models;
using BiddingServiceAPI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BiddingServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BiddingController : ControllerBase
    {
        private readonly IBiddingInterface _biddingService;
        private readonly ILogger<BiddingController> _logger;

        public BiddingController(IBiddingInterface biddingService, ILogger<BiddingController> logger)
        {
            _biddingService = biddingService;
            _logger = logger;

            // Log service information
            LogServiceInformation();
        }

        private void LogServiceInformation()
        {
            // Get hostname and IP address
            var hostName = System.Net.Dns.GetHostName();
            var ips = System.Net.Dns.GetHostAddresses(hostName);
            var ipAddress = ips.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();

            // Log information about service
            if (!string.IsNullOrEmpty(ipAddress))
            {
                _logger.LogInformation($"Bidding Service responding from {ipAddress}");
            }
            else
            {
                _logger.LogWarning("Unable to determine the IP address of the host.");
            }
        }

       

        [HttpPost]
        public ActionResult<string> AddBid(Bid bid)
        {
            _logger.LogInformation("Attempting to add bid");

            var result =  _biddingService.AddBid(bid);

            return Ok(result); // ikke altid "Ok(result)
        }

    }
}