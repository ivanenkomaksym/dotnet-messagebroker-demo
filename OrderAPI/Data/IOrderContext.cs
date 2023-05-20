using Common.Models;
using MongoDB.Driver;

namespace OrderAPI.Data
{
    public interface IOrderContext
    {
        IMongoCollection<Order> Orders { get; }
    }
}
