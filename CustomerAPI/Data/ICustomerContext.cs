using Common.Models;
using MongoDB.Driver;

namespace CustomerAPI.Data
{
    public interface ICustomerContext
    {
        IMongoCollection<Customer> Customers { get; }
    }
}
