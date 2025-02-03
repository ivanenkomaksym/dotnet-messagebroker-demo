using System.Text.Json;
using Common.Events;
using Common.Models;
using MassTransit;

namespace OrderProcessor.Discount
{
    public static class DiscountMessages
    {
        public static async Task SendAddUserCashback(this IPublishEndpoint publishEndpoint, ILogger logger, Order order, decimal cashback, DateTime validUntil)
        {
            var addUserCashback = new AddUserCashback
            {
                CustomerInfo = order.CustomerInfo,
                Cashback = cashback.ToString(),
                ValidUntil = validUntil
            };

            await publishEndpoint.Publish(addUserCashback);

            var message = JsonSerializer.Serialize(addUserCashback);
            logger.LogInformation($"Sent `AddUserCashback` event with content: {message}");
        }

        public static async Task SendSubUserCashback(this IPublishEndpoint publishEndpoint, ILogger logger, Order order, decimal cashback)
        {
            var subUserCashback = new SubUserCashback
            {
                CustomerInfo = order.CustomerInfo,
                Cashback = cashback.ToString()
            };

            await publishEndpoint.Publish(subUserCashback);

            var message = JsonSerializer.Serialize(subUserCashback);
            logger.LogInformation($"Sent `SubUserCashback` event with content: {message}");
        }

        // To be moved to Discount microservice
        public static decimal GetCashbackPercentageForUser(decimal totalPrice)
        {
            if (totalPrice < 50)
                return 0.05m;
            if (totalPrice < 100)
                return 0.1m;

            return 0.15m;
        }
    }
}