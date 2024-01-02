using MassTransit;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using Stock.API.Models;

namespace Stock.API.Consumer
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly ILogger<PaymentFailedEventConsumer> _logger;
        private readonly AppDbContext _appDbContext;

        public PaymentFailedEventConsumer(ILogger<PaymentFailedEventConsumer> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            foreach (var item in context.Message.OrderItems)
            {
                //var countFirst = await _appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId).Select(x => x.Count);
                //_logger.LogInformation($"Stock geri eklenmeden önceki miktar : {countFirst}");
                //hangi ürünün stoğu azaltılcak o stoğu bul.
                var stock = await _appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                if (stock != null)
                {
                    //dbdeki countu itemden gelen count kadar düşür
                    stock.Count += item.Count;
                }

                await _appDbContext.SaveChangesAsync();

                //var countLast = await _appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId).Select(x => x.Count);
                //_logger.LogInformation($"Stock geri eklendikten sonraki miktar : {countLast}");
            }

            _logger.LogInformation($" Buyer Id : {context.Message.BuyerId} Kullanıcısı bakiye yetersiz olduğu için stock {context.Message.OrderItems.Count} kadar geri eklendi");
        }
    }
}