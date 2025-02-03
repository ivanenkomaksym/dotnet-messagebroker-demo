using System.Text.Json;
using Common.Events;
using Common.Models.Payment;
using MassTransit;
using PaymentService.Repositories;

namespace PaymentService.Consumers
{
    internal class RefundPaymentConsumer : IConsumer<RefundPayment>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<TakePaymentConsumer> _logger;

        public RefundPaymentConsumer(IPublishEndpoint publishEndpoint, IPaymentRepository paymentRepository, ILogger<TakePaymentConsumer> logger)
        {
            _publishEndpoint = publishEndpoint;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RefundPayment> context)
        {
            // In
            var refundPayment = context.Message;
            var message = JsonSerializer.Serialize(refundPayment);
            _logger.LogInformation($"Received `RefundPayment` event with content: {message}");

            // Out

            var payment = await _paymentRepository.GetPaymentByOrderId(refundPayment.OrderId);
            payment.PaymentStatus = PaymentStatus.Refunding;
            await _paymentRepository.UpdatePayment(payment);

            await SchedulePaymentRefund(refundPayment);
        }

        private Task SchedulePaymentRefund(RefundPayment refundPayment)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(10 * 1000); // wait 10s

                var payment = await _paymentRepository.GetPaymentByOrderId(refundPayment.OrderId);
                payment.PaymentStatus = PaymentStatus.Refunded;
                await _paymentRepository.UpdatePayment(payment);

                var paymentRefundedEvent = new PaymentRefunded
                {
                    OrderId = payment.OrderId,
                    PaymentId = payment.Id,
                    CustomerInfo = payment.CustomerInfo,
                    PaymentInfo = payment.PaymentInfo,
                    PaymentStatus = payment.PaymentStatus
                };

                await _publishEndpoint.Publish(paymentRefundedEvent);

                var message = JsonSerializer.Serialize(paymentRefundedEvent);
                _logger.LogInformation($"Sent `PaymentRefunded` event with content: {message}");
            });
        }
    }
}