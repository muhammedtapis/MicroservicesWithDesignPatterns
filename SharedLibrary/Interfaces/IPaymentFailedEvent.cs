using MassTransit;
using SharedLibrary.Messages;

namespace SharedLibrary.Interfaces
{
    public interface IPaymentFailedEvent : CorrelatedBy<Guid>
    {
        public string Message { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}