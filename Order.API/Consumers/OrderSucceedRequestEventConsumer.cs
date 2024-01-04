using MassTransit;
using Order.API.Model;
using SharedLibrary.Events;
using SharedLibrary.Interfaces;

namespace Order.API.Consumers
{
    public class OrderSucceedRequestEventConsumer : IConsumer<IOrderSucceedRequestEvent>
    {
        private readonly ILogger<OrderSucceedRequestEventConsumer> _logger;
        private readonly AppDbContext _appDbContext;

        public OrderSucceedRequestEventConsumer(ILogger<OrderSucceedRequestEventConsumer> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        //OrderSucceedRequestEvent gelirse bu servise çalışcak alan

        public async Task Consume(ConsumeContext<IOrderSucceedRequestEvent> context)
        {
            //ilk önce paymentten gelen eventin içindeki orderi bul.
            var order = await _appDbContext.Orders.FindAsync(context.Message.OrderId);

            //eğer order boş değilse  statusunu değiştir ve  veritabanına güncelle yani,
            if (order != null)
            {
                order.Status = OrderStatus.Succeed;
                await _appDbContext.SaveChangesAsync();
                _logger.LogInformation($"Order : ({context.Message.OrderId}) status changed  : ({order.Status}) ");
            }
            else
            {
                _logger.LogError($"Order : ({context.Message.OrderId}) not found ");
            }
        }
    }
}