using SharedLibrary.Interfaces;

namespace SharedLibrary
{
    //bunun subscriberi OrderApi
    public class StockNotReservedEvent : IStockNotReservedEvent
    {
        public StockNotReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        //public int OrderrId { get; set; }

        //public string Message { get; set; } //olumsuz durumu geri orderApi ye dönerken mesaj da göndermek isteyebiliriz.
        //public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();
        public string Message { get; set; }

        public Guid CorrelationId { get; } //sadece get olduğu için ctorda verdik
    }
}