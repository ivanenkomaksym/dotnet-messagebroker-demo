﻿namespace OrderAPI
{
    public interface IOrderService
    {
        public Task CreateOrder(Order order);
    }
}
