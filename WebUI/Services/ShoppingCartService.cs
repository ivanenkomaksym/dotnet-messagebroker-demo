using Common.Extensions;
using Common.Models;
using WebUI.Models;
using WebUI.Routing;

namespace WebUI.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly HttpClient _client;
        private readonly string _environmentRoutePrefix;

        public ShoppingCartService(HttpClient client, IEnvironmentRouter environmentRouter)
        {
            _client = client;
            _environmentRoutePrefix = environmentRouter.GetShoppingCartRoute();
        }

        public async Task<bool> Checkout(Guid customerId)
        {
            var response = await _client.PostAsJson($"{_environmentRoutePrefix}/{customerId}", "");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            return true;
        }

        public async Task CreateShoppingCart(ShoppingCartModel shoppingCart)
        {
            var response = await _client.PostAsJson($"{_environmentRoutePrefix}", shoppingCart);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public async Task<bool> DeleteShoppingCart(Guid customerId)
        {
            var response = await _client.DeleteAsync($"{_environmentRoutePrefix}/{customerId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return true;
        }

        public async Task<ShoppingCartModel?> GetShoppingCart(Guid customerId)
        {
            var response = await _client.GetAsync($"{_environmentRoutePrefix}/{customerId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.ReadContentAs<ShoppingCartModel>();
        }

        public async Task<ShoppingCartModel?> UpdateShoppingCart(ShoppingCartModel shoppingCart)
        {
            var response = await _client.PutAsJsonAsync($"{_environmentRoutePrefix}", shoppingCart);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }

            return await response.ReadContentAs<ShoppingCartModel>();
        }

        public async Task<ShoppingCartModel?> AddProductToCart(Guid customerId, ProductWithStock product, ushort quantity = 1)
        {
            var cart = await GetShoppingCart(customerId);
            if (cart == null)
                return null;

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
    }
}
