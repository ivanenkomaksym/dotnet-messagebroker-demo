using Common.Events;
using Common.Models;
using MassTransit;
using System.Text.Json;

namespace OrderProcessor.Discount
{
    public static class DiscountMessages
    {
        public static async Task SendAddUserCashback(this IPublishEndpoint publishEndpoint, ILogger logger, Order order, double cashback, DateTime validUntil)
        {
            var addUserCashback = new AddUserCashback
            {
                CustomerInfo = order.CustomerInfo,
                Cashback = cashback,
                ValidUntil = validUntil
            };

            await publishEndpoint.Publish(addUserCashback);

            var message = JsonSerializer.Serialize(addUserCashback);
            logger.LogInformation($"Sent `AddUserCashback` event with content: {message}");
        }

        public static async Task SendSubUserCashback(this IPublishEndpoint publishEndpoint, ILogger logger, Order order, double cashback)
        {
            var subUserCashback = new SubUserCashback
            {
                CustomerInfo = order.CustomerInfo,
                Cashback = cashback
            };

            await publishEndpoint.Publish(subUserCashback);

            var message = JsonSerializer.Serialize(subUserCashback);
            logger.LogInformation($"Sent `SubUserCashback` event with content: {message}");
        }

        // To be moved to Discount microservice
        public static double GetCashbackPercentageForUser(double totalPrice)
        {
            if (totalPrice < 50.0)
                return 0.05;
            if (totalPrice < 100.0)
                return 0.1;

            return 0.15;
        }
    }
}
