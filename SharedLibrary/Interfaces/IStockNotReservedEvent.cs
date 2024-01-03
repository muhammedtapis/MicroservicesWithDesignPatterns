using MassTransit;

namespace SharedLibrary.Interfaces
{
    public interface IStockNotReservedEvent : CorrelatedBy<Guid>
    {
        public string Message { get; set; }
    }
}