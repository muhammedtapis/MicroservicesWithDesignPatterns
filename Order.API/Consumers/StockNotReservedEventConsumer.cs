using MassTransit;
using Order.API.Model;
using SharedLibrary;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        private readonly ILogger<StockNotReservedEventConsumer> _logger;
        private readonly AppDbContext _appDbContext;

        public StockNotReservedEventConsumer(ILogger<StockNotReservedEventConsumer> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order = await _appDbContext.Orders.FindAsync(context.Message.OrderrId);
            if (order != null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Message;
                await _appDbContext.SaveChangesAsync();
                _logger.LogInformation($"Order : ({context.Message.OrderrId}) status changed  : ({order.Status}) ");
            }
            else
            {
                _logger.LogError($"{context.Message.OrderrId}) not found ");
            }
        }
    }
}