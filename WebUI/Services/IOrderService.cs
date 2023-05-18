using Common.Models;

namespace WebUI.Services
{
    public interface IOrderService
    {
        Task CreateOrder(Order order);
    }
}
