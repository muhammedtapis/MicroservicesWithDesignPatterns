using SharedLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Events
{
    public class PaymentSucceedEvent : IPaymentSucceedEvent
    {
        public PaymentSucceedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        //public int OrderrId { get; set; } //hangi siparişin bu payment ?
        //public string BuyerId { get; set; }

        public Guid CorrelationId { get; }
    }
}