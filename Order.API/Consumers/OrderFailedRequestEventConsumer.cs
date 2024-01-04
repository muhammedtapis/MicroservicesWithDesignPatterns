using MassTransit;
using Order.API.Model;
using SharedLibrary.Events;

namespace Order.API.Consumers
{
    public class OrderFailedRequestEventConsumer : IConsumer<OrderFailedRequestEvent>
    {
        private readonly ILogger<OrderFailedRequestEventConsumer> _logger;
        private readonly AppDbContext _appDbContext;

        public OrderFailedRequestEventConsumer(ILogger<OrderFailedRequestEventConsumer> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public async Task Consume(ConsumeContext<OrderFailedRequestEvent> context)
        {
            var order = await _appDbContext.Orders.FindAsync(context.Message.OrderId);
            if (order != null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Message;
                await _appDbContext.SaveChangesAsync();
                _logger.LogInformation($"Order : ({context.Message.OrderId}) status changed  : ({order.Status}) ");
            }
            else
            {
                _logger.LogError($"{context.Message.OrderId}) not found ");
            }
        }
    }
}