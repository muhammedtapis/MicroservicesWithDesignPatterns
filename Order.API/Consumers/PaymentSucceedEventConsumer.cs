using MassTransit;
using Order.API.Model;
using SharedLibrary;

namespace Order.API.Consumers
{
    public class PaymentSucceedEventConsumer : IConsumer<PaymentSucceedEvent>
    {
        private readonly ILogger<PaymentSucceedEventConsumer> _logger;
        private readonly AppDbContext _appDbContext;

        public PaymentSucceedEventConsumer(ILogger<PaymentSucceedEventConsumer> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public async Task Consume(ConsumeContext<PaymentSucceedEvent> context)
        {
            //ilk önce paymentten gelen eventin içindeki orderi bul.
            var order = await _appDbContext.Orders.FindAsync(context.Message.OrderrId);

            //eğer order boş değilse  statusunu değiştir ve  veritabanına güncelle yani,
            if (order != null)
            {
                order.Status = OrderStatus.Succeed;
                await _appDbContext.SaveChangesAsync();
                _logger.LogInformation($"Order : ({context.Message.OrderrId}) status changed  : ({order.Status}) ");
            }
            else
            {
                _logger.LogError($"Order : ({context.Message.OrderrId}) not found ");
            }
        }
    }
}