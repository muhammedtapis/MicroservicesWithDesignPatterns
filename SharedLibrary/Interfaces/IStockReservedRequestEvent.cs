using MassTransit;

namespace SharedLibrary.Interfaces
{
    public interface IStockReservedRequestEvent : CorrelatedBy<Guid>
    {
        //payment servise payment bilgilerini yolla
        public PaymentMessage Payment { get; set; }

        //başarısız olursa orderitem a göre tersine transaction yapcak
        public List<OrderItemMessage> OrderItems { get; set; }

        public string BuyerId { get; set; }
    }
}