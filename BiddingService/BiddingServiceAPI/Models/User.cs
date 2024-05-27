using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BiddingServiceAPI.Models
{
    public class User
    {
        [BsonId]
        public Guid _id { get; set; }
        public string username { get; set; }
    }
}