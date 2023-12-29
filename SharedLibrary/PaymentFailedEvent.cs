using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class PaymentFailedEvent
    {
        public int OrderId { get; set; } //hangi siparişin bu payment ?
        public int BuyerId { get; set; }
        public string Message { get; set; }
    }
}