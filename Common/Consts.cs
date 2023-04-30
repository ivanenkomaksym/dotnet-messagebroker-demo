namespace Common
{
    public static class Consts
    {
        public const string OrderQueue = "queue.order";

        public static string GetOrderStatusQueueName(string targetService)
        {
            return $"queue.order.status.{targetService}";
        }

        public static string GetCustomerStatusQueueName(string targetService)
        {
            return $"queue.customer.status.{targetService}";
        }

        public const string OrderStatusExchange = "exchange.order.status";
        public const string OrderStatusBindingKey = "order.status.#";
        public const string OrderStatusPaid = "order.status.paid";

        public const string CustomerStatusExchange = "exchange.customer.status";
        public const string CustomerStatusBindingKey = "customer.status.#";
        public const string CustomerStatusRegistered = "customer.status.registered";
    }
}