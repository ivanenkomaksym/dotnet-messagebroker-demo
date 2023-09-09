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

            // Out
            _ = SchedulePayment(takePayment);
        }

        private Task SchedulePayment(TakePayment takePayment)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(10 * 1000); // wait 10s

                await HandlePayment(takePayment);
            });
        }

        private async Task HandlePayment(TakePayment takePayment)
        {
            var paymentStatus = PaymentStatus.Unpaid;
            var paidAmount = 0.0m;
            switch (takePayment.PaymentInfo.PaymentMethod)
            {
                case PaymentMethod.CreditCard_AlwaysExpire:
                    paymentStatus = PaymentStatus.Expired;
                    break;
                case PaymentMethod.Crypto:
                    paymentStatus = PaymentStatus.Paid;
                    paidAmount = decimal.Parse(takePayment.ToBePaidAmount);
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

            var paymentResultEvent = new PaymentResult
            {
                OrderId = payment.OrderId,
                PaymentId = payment.Id,
                CustomerInfo = payment.CustomerInfo,
                PaymentInfo = payment.PaymentInfo,
                PaymentStatus = payment.PaymentStatus
            };

            await _publishEndpoint.Publish(paymentResultEvent);

            var message = JsonSerializer.Serialize(paymentResultEvent);
            _logger.LogInformation($"Sent `PaymentResult` event with content: {message}");
        }
    }
}
