using SharedLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Events
{
    //statemachine orderservis için gönderdiği event
    public class OrderSucceedRequestEvent : IOrderSucceedRequestEvent
    {
        public int OrderId { get; set; }
    }
}