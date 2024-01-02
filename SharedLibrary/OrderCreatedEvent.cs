using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    //Subscriberi OrderApi
    public class OrderCreatedEvent
    {
        public int OrderrId { get; set; } //hangi order
        public string BuyerId { get; set; } //hangi kullanıcıya ait
        public PaymentMessage Payment { get; set; }

        public List<OrderItemMessage> OrderItemMessages { get; set; } = new List<OrderItemMessage>();
    }
}