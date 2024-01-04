using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Messages;
using Stock.API.Models;

namespace Stock.API.Consumer
{
    public class StockRollBackRequestMessageConsumer : IConsumer<IStockRollBackRequestMessage>
    {
        private readonly ILogger<StockRollBackRequestMessageConsumer> _logger;
        private readonly AppDbContext _appDbContext;

        public StockRollBackRequestMessageConsumer(ILogger<StockRollBackRequestMessageConsumer> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public async Task Consume(ConsumeContext<IStockRollBackRequestMessage> context)
        {
            foreach (var item in context.Message.OrderItems)
            {
                var countFirst = _appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId).Result.Count;
                _logger.LogInformation($"({item.ProductId}) ürününün stok geri eklenmeden önceki miktarı : {countFirst}");
                //hangi ürünün stoğu azaltılcak o stoğu bul.
                var stock = await _appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                if (stock != null)
                {
                    //dbdeki countu itemden gelen count kadar düşür
                    stock.Count += item.Count;
                }

                await _appDbContext.SaveChangesAsync();

                var countLast = _appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId).Result.Count;
                _logger.LogInformation($"({item.ProductId}) ürününün stok geri eklendikten sonraki miktarı : {countLast}");
            }

            _logger.LogInformation($"Bakiye yetersiz olduğu için Item : {context.Message.OrderItems.Count} kadar geri eklendi");
        }
    }
}