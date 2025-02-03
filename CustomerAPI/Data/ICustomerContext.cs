using Common.Models;
using MongoDB.Driver;

namespace CustomerAPI.Data
{
    internal interface ICustomerContext
    {
        IMongoCollection<Customer> Customers { get; }
    }
}