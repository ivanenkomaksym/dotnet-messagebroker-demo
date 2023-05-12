using MongoDB.Driver;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Entities;

namespace ShoppingCartAPI.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private IShoppingCartContext _context;

        public ShoppingCartRepository(IShoppingCartContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCart> GetShoppingCart(Guid customerId)
        {
            return await _context
                           .ShoppingCarts
                           .Find(c => c.CustomerId == customerId)
                           .FirstOrDefaultAsync();
        }

        public async Task CreateShoppingCart(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.InsertOneAsync(shoppingCart);
        }

        public async Task<bool> UpdateShoppingCart(ShoppingCart shoppingCart)
        {
            var updateResult = await _context
                                        .ShoppingCarts
                                        .ReplaceOneAsync(filter: c => c.Id == shoppingCart.Id, replacement: shoppingCart);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteShoppingCart(Guid customerId)
        {
            var filter = Builders<ShoppingCart>.Filter.Eq(c => c.CustomerId, customerId);

            DeleteResult deleteResult = await _context
                                                .ShoppingCarts
                                                .DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public Task<bool> Checkout(Guid customerId)
        {
            // TODO: Implement it
            return Task.FromResult(true);
        }
    }
}
