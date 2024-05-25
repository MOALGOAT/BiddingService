using System;
using MongoDB.Bson.Serialization.Attributes;


namespace BiddingServiceAPI.Models
{
    public class Bid
    {
     [BsonId]

        public Guid _id { get; set; }
        public User user { get; set; }
        public float bidPrice { get; set; }
        public DateTime? dateTime { get; set; } = DateTime.Now;
        public Guid auctionId { get; set; }
    }

  
}