using Common.Models;
using MongoDB.Driver;

namespace OrderAPI.Data
{
    internal interface IOrderContextSeed
    {
        public Task SeedData(IMongoCollection<Order> orderCollection);
    }
}