using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    //bunun subscriberi OrderApi
    public class StockNotReservedEvent
    {
        public int OrderId { get; set; }

        public string Message { get; set; } //olumsuz durumu geri orderApi ye dönerken mesaj da göndermek isteyebiliriz.
    }
}