using SharedLibrary.Interfaces;
using SharedLibrary.Messages;

namespace SharedLibrary.Events
{
    public class PaymentFailedEvent : IPaymentFailedEvent
    {
        public PaymentFailedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string Message { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();

        public Guid CorrelationId { get; }
    }
}