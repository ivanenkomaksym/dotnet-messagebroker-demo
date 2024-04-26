using Common.Models;
using WebUI.Models;

namespace WebUI.Services
{
    public class StubShoppingCartService : IShoppingCartService
    {
        public async Task<ShoppingCartModel?> AddProductToCart(Guid customerId, ProductWithStock product, ushort quantity = 1)
        {
            var cart = await GetShoppingCart(customerId);
            ArgumentNullException.ThrowIfNull(cart);

            var items = cart.Items.Where(x => x.ProductId == product.Id);
            if (items.Any())
            {
                items.First().Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new ShoppingCartItemModel
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductPrice = product.DiscountedPrice,
                    Quantity = quantity,
                    ImageFile = product.ImageFile,
                    AvailableOnStock = product.AvailableOnStock
                });
            }

            return await UpdateShoppingCart(cart);
        }

        public Task<bool> Checkout(Guid customerId)
        {
            throw new NotImplementedException();
        }

        public Task CreateShoppingCart(ShoppingCartModel shoppingCart)
        {
            ShoppingCarts.Add(shoppingCart);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteShoppingCart(Guid customerId)
        {
            var cart = ShoppingCarts.Find(x => x.CustomerId == customerId);
            var found = cart != null;
            if (cart != null)
                ShoppingCarts.Remove(cart);

            return Task.FromResult(found);
        }

        public async Task<ShoppingCartModel?> GetShoppingCart(Guid customerId)
        {
            var shoppingCart = ShoppingCarts.FirstOrDefault(x => x.CustomerId == customerId);
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCartModel { Id = Guid.NewGuid(), CustomerId = customerId };
                await CreateShoppingCart(shoppingCart);
            }

            return shoppingCart;
        }

        public Task<ShoppingCartModel?> UpdateShoppingCart(ShoppingCartModel shoppingCart)
        {
            var foundShoppingCart = ShoppingCarts.Find(c => c.Id == shoppingCart.Id);
            if (foundShoppingCart == null)
                return Task.FromResult<ShoppingCartModel?>(null);

            var index = ShoppingCarts.IndexOf(foundShoppingCart);
            if (index != -1)
            {
                ShoppingCarts[index] = shoppingCart;
            }

            return Task.FromResult<ShoppingCartModel?>(shoppingCart);
        }

        private readonly List<ShoppingCartModel> ShoppingCarts = new();
    }
}
