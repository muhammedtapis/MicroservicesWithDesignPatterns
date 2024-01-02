using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    //Subscriberi PaymentApi
    //öncelikle  bu eventi kim dinleyecek subscribe olacak ona bak ?
    //payment servis dinleyecek subscribbe olacak o yüzden ödeme bilgilerini göndermen lazım.
    public class StockReservedEvent
    {
        public int OrderrId { get; set; } //hangi sipariş için
        public string BuyerId { get; set; }
        public PaymentMessage PaymentMessage { get; set; }

        //orderitem almamızın sebebi StockReservedEventi Payment yolladığımızda payment fail dönerse stoktan düştüğü ürünleri tekrar düzeltmek için
        //tersine transaction yapılması için .
        public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();
    }
}