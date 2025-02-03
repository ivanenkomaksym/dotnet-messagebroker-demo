using Common.Models;
using MongoDB.Driver;

namespace OrderCommon.Data
{
    public interface IOrderContext
    {
        IMongoCollection<Order> Orders { get; }

        public abstract Task InitAsync();
    }
}