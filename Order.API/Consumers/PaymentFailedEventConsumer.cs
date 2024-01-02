using MassTransit;
using Order.API.Model;
using SharedLibrary;

namespace Order.API.Consumers
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
            var order = await _appDbContext.Orders.FindAsync(context.Message.OrderrId);
            if (order != null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Message;
                await _appDbContext.SaveChangesAsync();
                _logger.LogInformation($"Order : ({context.Message.OrderrId}) status changed  : ({order.Status}) ");
                //_appDbContext.Orders.Remove(order);
            }
            else
            {
                _logger.LogError($"{context.Message.OrderrId}) not found ");
            }
        }
    }
}