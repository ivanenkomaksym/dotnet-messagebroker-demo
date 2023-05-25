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

            var payment = await _paymentRepository.CreatePayment(new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = takePayment.OrderId,
                CustomerInfo = takePayment.CustomerInfo,
                PaymentInfo = takePayment.PaymentInfo,
                PaymentStatus = PaymentStatus.Unpaid
            });

            // Out
            // TODO: send PaymentTaken message if successful
        }
    }
}
