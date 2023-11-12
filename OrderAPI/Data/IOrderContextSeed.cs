using Common.Models;
using MongoDB.Driver;

namespace OrderAPI.Data
{
    public interface IOrderContextSeed
    {
        public Task SeedData(IMongoCollection<Order> orderCollection);
    }
}
