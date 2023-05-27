using System.Text.Json;
using Common.Events;
using Common.Models.Payment;
using PaymentService.Repositories;
using MassTransit;

namespace PaymentService.Consumers
{
    internal class TakePaymentConsumer : IConsumer<TakePayment>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<TakePaymentConsumer> _logger;

        public TakePaymentConsumer(IPublishEndpoint publishEndpoint, IPaymentRepository paymentRepository, ILogger<TakePaymentConsumer> logger)
        {
            _publishEndpoint = publishEndpoint;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TakePayment> context)
        {
            // In
            var takePayment = context.Message;
            var message = JsonSerializer.Serialize(takePayment);
            _logger.LogInformation($"Received `TakePayment` event with content: {message}");

            // TODO: Hardcode different PaymentStatus depending on PaymentMethod
            var paymentStatus = PaymentStatus.Unpaid;
            var paidAmount = 0.0;
            switch (takePayment.PaymentInfo.PaymentMethod)
            {
                case PaymentMethod.CreditCard_AlwaysExpire:
                    paymentStatus = PaymentStatus.Expired;
                    break;
                case PaymentMethod.Crypto:
                    paymentStatus = PaymentStatus.Paid;
                    paidAmount = takePayment.ToBePaidAmount;
                    break;
                case PaymentMethod.PayPal_AlwaysFail:
                    paymentStatus = PaymentStatus.Failed;
                    break;
            }

            // Payment can already be found for known orders with failed previously payments
            var payment = await _paymentRepository.GetPaymentByOrderId(takePayment.OrderId);
            if (payment != null)
            {
                payment.PaymentStatus = paymentStatus;
                await _paymentRepository.UpdatePayment(payment);
            }
            else
            {
                payment = await _paymentRepository.CreatePayment(new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = takePayment.OrderId,
                    CustomerInfo = takePayment.CustomerInfo,
                    PaymentInfo = takePayment.PaymentInfo,
                    CreatedOn = DateTime.UtcNow,
                    PaymentStatus = paymentStatus,
                    PaidAmount = paidAmount
                });
            }

            // Out
            var paymentResultEvent = new PaymentResult
            {
                OrderId = payment.OrderId,
                PaymentId = payment.Id,
                CustomerInfo = payment.CustomerInfo,
                PaymentInfo = payment.PaymentInfo,
                PaymentStatus = payment.PaymentStatus
            };

            await _publishEndpoint.Publish(paymentResultEvent);

            message = JsonSerializer.Serialize(paymentResultEvent);
            _logger.LogInformation($"Sent `PaymentResult` event with content: {message}");
        }
    }
}
