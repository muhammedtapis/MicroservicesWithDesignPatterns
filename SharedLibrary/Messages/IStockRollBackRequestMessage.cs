using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Messages
{
    public interface IStockRollBackRequestMessage
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}