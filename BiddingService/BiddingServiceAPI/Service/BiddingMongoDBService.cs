using BiddingServiceAPI.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BiddingServiceAPI.Service
{
    public interface IBiddingInterface
    {
        Task<Bid?> GetBid(Guid _id);
        Task<IEnumerable<Bid>?> GetBidList();
        Task<Guid> AddBid(Bid bid);
        Task<long> UpdateBid(Bid bid);
        Task<long> DeleteBid(Guid _id);
    }

    public class BiddingMongoDBService : IBiddingInterface
    {
        private readonly ILogger<BiddingMongoDBService> _logger;
        private readonly IMongoCollection<Bid> _biddingCollection;

        public BiddingMongoDBService(ILogger<BiddingMongoDBService> logger, MongoDBContext dbContext, IConfiguration configuration)
        {
            var collectionName = configuration["biddingCollectionName"];
            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ApplicationException("BiddingCollectionName is not configured.");
            }

            _logger = logger;
            _biddingCollection = dbContext.GetCollection<Bid>(collectionName);
            _logger.LogInformation($"Collection name: {collectionName}");
        }

        public async Task<Bid?> GetBid(Guid _id)
        {
            var filter = Builders<Bid>.Filter.Eq(x => x._id, _id);
            return await _biddingCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Bid>?> GetBidList()
        {
            return await _biddingCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Guid> AddBid(Bid bid)
        {
            bid._id = Guid.NewGuid();
            await _biddingCollection.InsertOneAsync(bid);
            return bid._id;
        }

        public async Task<long> UpdateBid(Bid bid)
        {
            var filter = Builders<Bid>.Filter.Eq(x => x._id, bid._id);
            var result = await _biddingCollection.ReplaceOneAsync(filter, bid);
            return result.ModifiedCount;
        }

        public async Task<long> DeleteBid(Guid _id)
        {
            var filter = Builders<Bid>.Filter.Eq(x => x._id, _id);
            var result = await _biddingCollection.DeleteOneAsync(filter);
            return result.DeletedCount;
        }
    }
}
