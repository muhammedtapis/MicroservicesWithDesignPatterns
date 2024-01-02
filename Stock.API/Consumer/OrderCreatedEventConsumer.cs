using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using Stock.API.Models;

namespace Stock.API.Consumer
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        //kuyruktan her bir mesajı dinlediğimizde burası çalışacak.
        private readonly AppDbContext _appDbContext;

        private readonly ILogger<OrderCreatedEvent> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider; //send ile göndercez kuyruk ismi verip
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(AppDbContext appDbContext, ILogger<OrderCreatedEvent> logger, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var stockResult = new List<bool>();

            foreach (var item in context.Message.OrderItemMessages)
            {
                //eğer dbdeki productid ile orderCreatedEvent den gelen product id eşit ve dbdeki count ordercreatedeventten gelen counttan büyükse
                //result true dön değilse fals dön ve stockResult ekle
                stockResult.Add(await _appDbContext.Stocks.AnyAsync(x => x.ProductId == item.ProductId && x.Count > item.Count));
            }

            if (stockResult.All(x => x.Equals(true)))
            {
                //eğer resulttaki tüm değeler true ise stock durumu okeydir önce veritabanından bu değerleri eksilt.
                //o zaman StockReservedEvent fırlatıcaz.
                foreach (var item in context.Message.OrderItemMessages)
                {
                    //hangi ürünün stoğu azaltılcak o stoğu bul.
                    var stock = await _appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                    if (stock != null)
                    {
                        //dbdeki countu itemden gelen count kadar düşür
                        stock.Count -= item.Count;
                    }

                    await _appDbContext.SaveChangesAsync();
                }

                _logger.LogInformation($"Stock was reserved for Buyer Id : {context.Message.BuyerId}");

                var sendEndPoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettingsConst.StockReservedEventQueueName}"));

                StockReservedEvent stockReservedEvent = new StockReservedEvent()
                {
                    PaymentMessage = context.Message.Payment,
                    BuyerId = context.Message.BuyerId,
                    OrderrId = context.Message.OrderrId,
                    OrderItems = context.Message.OrderItemMessages
                };

                //eğer stock durumu başarılı mevcutsa her şey okeyse paymenta eventimizi gönderiyoruz.
                //yukarıdaki oluşturduğumuz endpointe
                await sendEndPoint.Send(stockReservedEvent);
            }
            else
            {
                //eğer durum başarısız stock mevcut değilse bu sefer send etmiycez pubish edicez çünkü bu eventi birden fazla servis dinleyebilir.
                await _publishEndpoint.Publish(new StockNotReservedEvent()
                {
                    OrderrId = context.Message.OrderrId,
                    Message = "Stok yetersiz",
                    OrderItems = context.Message.OrderItemMessages
                });

                _logger.LogInformation($" Buyer Id : {context.Message.BuyerId} Kullanıcısı için stock yetersiz ");
            }
        }
    }
}