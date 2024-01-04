using SharedLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Events
{
    //orderservise statemacgine gönercek bu eventi
    public class OrderFailedRequestEvent : IOrderFailedRequestEvent
    {
        public int OrderId { get; set; }
        public string Message { get; set; }
    }
}