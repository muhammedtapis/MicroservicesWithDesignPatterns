using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class PaymentFailedEvent
    {
        public int OrderrId { get; set; } //hangi siparişin bu payment ?
        public string BuyerId { get; set; }
        public string Message { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();
    }
}