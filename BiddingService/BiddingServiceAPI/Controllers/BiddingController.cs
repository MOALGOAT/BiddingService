using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BiddingServiceAPI.Models;
using BiddingServiceAPI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;


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

            try
            {
                var result = _biddingService.AddBid(bid);

                if (result == null) // eller en anden betingelse der indikerer fejl
                {
                    var errorMessage = "Failed to add bid: result is null";
                    _logger.LogError(errorMessage);
                    return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while adding the bid: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
            }
        }


    }
}