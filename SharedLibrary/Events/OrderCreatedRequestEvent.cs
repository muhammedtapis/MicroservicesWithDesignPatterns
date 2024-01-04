using SharedLibrary.Interfaces;
using SharedLibrary.Messages;

namespace SharedLibrary.Events
{
    //Subscriberi StockApi
    public class OrderCreatedRequestEvent : IOrderCreatedRequestEvent
    {
        //    public int OrderrId { get; set; } //hangi order
        //    public string BuyerId { get; set; } //hangi kullanıcıya ait
        //    public PaymentMessage Payment { get; set; }

        public OrderCreatedRequestEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();

        public Guid CorrelationId { get; }
    }
}