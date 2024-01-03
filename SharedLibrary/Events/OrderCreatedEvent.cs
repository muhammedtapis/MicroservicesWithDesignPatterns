using SharedLibrary.Interfaces;

namespace SharedLibrary.Events
{
    //Subscriberi OrderApi
    public class OrderCreatedEvent : IOrderCreatedEvent
    {
        //    public int OrderrId { get; set; } //hangi order
        //    public string BuyerId { get; set; } //hangi kullanıcıya ait
        //    public PaymentMessage Payment { get; set; }

        public OrderCreatedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();

        public Guid CorrelationId { get; }
    }
}