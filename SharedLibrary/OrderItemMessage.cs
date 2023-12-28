using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class OrderItemMessage
    {
        public int ProductId { get; set; } //hangi producta ait bu order item
        public int Count { get; set; } //stock.API ilgili ProductID sahip productttan kaç adet azaltacak gerekli
    }
}