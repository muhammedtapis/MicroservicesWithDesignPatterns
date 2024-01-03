using MassTransit;
using SharedLibrary.Events;

namespace Payment.API.Consumers
{
    public class StockReservedRequestEventConsumer : IConsumer<StockReservedRequestEvent>
    {
        private readonly ILogger<StockReservedRequestEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedRequestEventConsumer(ILogger<StockReservedRequestEventConsumer> logger, IPublishEndpoint publishendpoint)
        {
            _logger = logger;
            _publishEndpoint = publishendpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedRequestEvent> context)
        {
            //müşteri bakiyesi belirle örnek
            var balance = 3000m;

            //müşterinin bakiyesi paymenttan gelen total fiyattan büyükse ödeme başarılı olcak
            if (balance > context.Message.Payment.TotalPrice)
            {
                _logger.LogInformation($"Kullanıcı Id : {context.Message.CorrelationId} , {context.Message.Payment.TotalPrice} TL kartınızdan çekildi");
                //publish et olumlu eventi
                await _publishEndpoint.Publish(new PaymentSucceedEvent(context.Message.CorrelationId));
            }
            else
            {
                _logger.LogInformation($"Kullanıcı Id : {context.Message.BuyerId} , {context.Message.Payment.TotalPrice} TL kartınızdan çekilemedi bakiye yetersiz.");

                //publish et olumlu eventi
                await _publishEndpoint.Publish(new PaymentFailedEvent(context.Message.CorrelationId)
                {
                    Message = "Hesabınızdaki bakiye yetersiz.",
                    OrderItems = context.Message.OrderItems,
                });
            }
        }
    }
}