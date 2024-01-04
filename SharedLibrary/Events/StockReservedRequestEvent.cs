using SharedLibrary.Interfaces;
using SharedLibrary.Messages;

namespace SharedLibrary.Events
{
    public class StockReservedRequestEvent : IStockReservedRequestEvent
    {
        //ORderStateMAchine in Payment servisi tetiklemek için gönderdiği request event
        public StockReservedRequestEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public PaymentMessage Payment { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }

        public Guid CorrelationId { get; }
        public string BuyerId { get; set; }
    }
}