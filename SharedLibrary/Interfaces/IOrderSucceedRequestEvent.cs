using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Interfaces
{
    //correlate miras almamıza gerek yok çnükü sipariş bitti artık final olcak state
    public interface IOrderSucceedRequestEvent
    {
        public int OrderId { get; set; }
    }
}