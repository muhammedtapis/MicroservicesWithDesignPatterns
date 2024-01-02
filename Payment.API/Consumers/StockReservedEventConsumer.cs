using MassTransit;
using SharedLibrary;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly ILogger<StockReservedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishendpoint)
        {
            _logger = logger;
            _publishEndpoint = publishendpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            //müşteri bakiyesi belirle örnek
            var balance = 3000m;

            //müşterinin bakiyesi paymenttan gelen total fiyattan büyükse ödeme başarılı olcak
            if (balance > context.Message.PaymentMessage.TotalPrice)
            {
                _logger.LogInformation($"Kullanıcı Id : {context.Message.BuyerId} , {context.Message.PaymentMessage.TotalPrice} TL kartınızdan çekildi");
                var ordr = context.Message.OrderrId;
                //publish et olumlu eventi
                await _publishEndpoint.Publish(new PaymentSucceedEvent()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderrId = context.Message.OrderrId,
                });
            }
            else
            {
                _logger.LogInformation($"Kullanıcı Id : {context.Message.BuyerId} , {context.Message.PaymentMessage.TotalPrice} TL kartınızdan çekilemedi bakiye yetersiz.");

                //publish et olumlu eventi
                await _publishEndpoint.Publish(new PaymentFailedEvent()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderrId = context.Message.OrderrId,
                    Message = "Hesabınızdaki bakiye yetersiz.",
                    OrderItems = context.Message.OrderItems,
                });
            }
        }
    }
}