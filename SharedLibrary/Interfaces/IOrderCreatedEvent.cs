using SharedLibrary.Messages;

namespace SharedLibrary.Interfaces
{
    public interface IOrderCreatedEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }

        public List<OrderItemMessage> OrderItems { get; set; }

        public PaymentMessage Payment { get; set; }
    }
}