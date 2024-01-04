using SharedLibrary.Interfaces;
using SharedLibrary.Messages;

namespace SharedLibrary.Events
{
    public class OrderCreatedEvent : IOrderCreatedEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();
        public PaymentMessage Payment { get; set; }
    }
}