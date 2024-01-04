using SharedLibrary.Interfaces;
using SharedLibrary.Messages;

namespace SharedLibrary.Events
{
    //Subscriberi PaymentApi
    //öncelikle  bu eventi kim dinleyecek subscribe olacak ona bak ?
    //payment servis dinleyecek subscribbe olacak o yüzden ödeme bilgilerini göndermen lazım.
    public class StockReservedEvent : IStockReservedEvent
    {
        public StockReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        //public int OrderrId { get; set; } //hangi sipariş için
        //public string BuyerId { get; set; }
        //public PaymentMessage PaymentMessage { get; set; }

        ////orderitem almamızın sebebi StockReservedEventi Payment yolladığımızda payment fail dönerse stoktan düştüğü ürünleri tekrar düzeltmek için
        ////tersine transaction yapılması için .
        //public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();
        public List<OrderItemMessage> OrderItems { get; set; }

        public Guid CorrelationId { get; }
    }
}