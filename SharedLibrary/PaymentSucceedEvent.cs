using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class PaymentSucceedEvent
    {
        public int OrderrId { get; set; } //hangi siparişin bu payment ?
        public string BuyerId { get; set; }
    }
}