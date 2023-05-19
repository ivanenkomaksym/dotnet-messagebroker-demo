using Common.Models;
using CustomerAPI.Data;
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

        public async Task<Customer> GetCustomerById(Guid customerId)
        {
            var matchId = Builders<Customer>.Filter.Eq(c => c.Id, customerId);

            return await _context
                            .Customers
                            .Find(matchId)
                            .FirstOrDefaultAsync();
        }

        public async Task<Customer> Authenticate(string email, string password)
        {
            var matchEmail = Builders<Customer>.Filter.Eq(c => c.Email, email);
            var matchPassword = Builders<Customer>.Filter.Eq(c => c.Password, password);
            var matchEmailAndPassword = Builders<Customer>.Filter.And(matchEmail, matchPassword);

            return await _context
                            .Customers
                            .Find(matchEmailAndPassword)
                            .FirstOrDefaultAsync();
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            var c = await Authenticate(customer.Email, customer.Password);
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
