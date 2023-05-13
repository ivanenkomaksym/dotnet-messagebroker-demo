using CustomerAPI.Entities;
using MongoDB.Driver;

namespace CustomerAPI.Data
{
    public interface ICustomerContext
    {
        IMongoCollection<Customer> Customers { get; }
    }
}
