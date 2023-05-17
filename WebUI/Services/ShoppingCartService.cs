using WebUI.Extensions;
using WebUI.Models;

namespace WebUI.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly HttpClient _client;

        public ShoppingCartService(HttpClient client)
        {
            _client = client;
        }

        public async Task<bool> Checkout(Guid customerId)
        {
            var response = await _client.PostAsJson($"/gateway/ShoppingCart/{customerId}", "");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong when calling api.");
            }
            return true;
        }

        public async Task CreateShoppingCart(ShoppingCartModel shoppingCart)
        {
            var response = await _client.PostAsJson($"/gateway/ShoppingCart", shoppingCart);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong when calling api.");
            }
        }

        public async Task<bool> DeleteShoppingCart(Guid customerId)
        {
            var response = await _client.DeleteAsync($"/gateway/ShoppingCart/{customerId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<ShoppingCartModel> GetShoppingCart(Guid customerId)
        {
            var response = await _client.GetAsync($"/gateway/ShoppingCart/{customerId}");
            return await response.ReadContentAs<ShoppingCartModel>();
        }

        public async Task<ShoppingCartModel> UpdateShoppingCart(ShoppingCartModel shoppingCart)
        {
            var response = await _client.PutAsJsonAsync($"/gateway/ShoppingCart", shoppingCart);
            return await response.ReadContentAs<ShoppingCartModel>();
        }

        public async Task<ShoppingCartModel> AddProductToCart(Guid customerId, CatalogModel product)
        {
            var cart = await GetShoppingCart(customerId);

            var items = cart.Items.Where(x => x.ProductId == product.Id);
            if (items.Any())
            {
                items.First().Quantity++;
            }
            else
            {
                cart.Items.Add(new ShoppingCartItemModel
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    Quantity = 1
                });
            }

            return await UpdateShoppingCart(cart);
        }
    }
}
