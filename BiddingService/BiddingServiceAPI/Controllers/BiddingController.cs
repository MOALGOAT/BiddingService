using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BiddingServiceAPI.Models;
using BiddingServiceAPI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpGet("{_id}")]
        public async Task<ActionResult<Bid>> GetBid(Guid _id)
        {
            _logger.LogInformation($"Attempting to retrieve bid with ID: {_id}");

            var bid = await _biddingService.GetBid(_id);
            if (bid == null)
            {
                _logger.LogWarning($"Bid with ID {_id} not found");
                return NotFound();
            }

            _logger.LogInformation($"Bid with ID {_id} retrieved successfully");
            return bid;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bid>>> GetBidList()
        {
            _logger.LogInformation("Attempting to retrieve bid list");

            var bidList = await _biddingService.GetBidList();
            if (bidList == null)
            {
                _logger.LogError("Bid list is null");
                throw new ApplicationException("The list is null");
            }

            _logger.LogInformation("Bid list retrieved successfully");
            return Ok(bidList);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> AddBid(Bid bid)
        {
            _logger.LogInformation("Attempting to add bid");

            var _id = await _biddingService.AddBid(bid);

            _logger.LogInformation($"Bid added with ID: {_id}");
            return CreatedAtAction(nameof(GetBid), new { _id = _id }, _id);
        }

        [HttpPut("{_id}")]
        public async Task<IActionResult> UpdateBid(Guid _id, Bid bid)
        {
            _logger.LogInformation($"Attempting to update bid with ID: {_id}");

            if (_id != bid._id)
            {
                _logger.LogWarning("ID in URL does not match ID in request body");
                return BadRequest();
            }

            var result = await _biddingService.UpdateBid(bid);
            if (result == 0)
            {
                _logger.LogWarning($"Bid with ID {_id} not found");
                return NotFound();
            }

            _logger.LogInformation($"Bid with ID {_id} updated successfully");
            return NoContent();
        }

        [HttpDelete("{_id}")]
        public async Task<IActionResult> DeleteBid(Guid _id)
        {
            _logger.LogInformation($"Attempting to delete bid with ID: {_id}");

            var result = await _biddingService.DeleteBid(_id);
            if (result == 0)
            {
                _logger.LogWarning($"Bid with ID {_id} not found");
                return NotFound();
            }

            _logger.LogInformation($"Bid with ID {_id} deleted successfully");
            return Ok();
        }
    }
}
