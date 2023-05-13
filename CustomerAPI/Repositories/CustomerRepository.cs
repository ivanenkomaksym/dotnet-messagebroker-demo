using CustomerAPI.Data;
using CustomerAPI.Entities;
using MongoDB.Driver;

namespace CustomerAPI.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ICustomerContext _context;

        public CustomerRepository(ICustomerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await _context
                            .Customers
                            .Find(c => true)
                            .ToListAsync();
        }

        public async Task<Customer> GetCustomerByEmail(string email)
        {
            FilterDefinition<Customer> filter = Builders<Customer>.Filter.Eq(c => c.Email, email);

            return await _context
                            .Customers
                            .Find(filter)
                            .FirstOrDefaultAsync();
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            var c = await GetCustomerByEmail(customer.Email);
            if (c != null)
                return null;

            await _context.Customers.InsertOneAsync(customer);
            return customer;
        }

        public async Task<bool> DeleteCustomer(Guid customerId)
        {
            FilterDefinition<Customer> filter = Builders<Customer>.Filter.Eq(c => c.Id, customerId);

            DeleteResult deleteResult = await _context
                                                .Customers
                                                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }
    }
}
