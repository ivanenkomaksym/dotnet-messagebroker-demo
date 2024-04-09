﻿using Common.Models.Payment;
using Common.Models;

namespace Common.Events
{
    public record RefundPayment
    {
        public Guid OrderId { get; init; }

        public required CustomerInfo CustomerInfo { get; init; }

        public required PaymentInfo PaymentInfo { get; init; }

        public required string TotalPrice { get; set; }
    }
}
