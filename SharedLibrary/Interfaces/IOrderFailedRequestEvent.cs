using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Interfaces
{
    //orderservise statemachine tarafından gidicek event
    public interface IOrderFailedRequestEvent
    {
        public int OrderId { get; set; }
        public string Message { get; set; }
    }
}