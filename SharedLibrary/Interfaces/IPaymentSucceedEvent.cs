using MassTransit;

namespace SharedLibrary.Interfaces
{
    public interface IPaymentSucceedEvent : CorrelatedBy<Guid>
    {
    }
}