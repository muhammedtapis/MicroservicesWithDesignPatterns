using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Messages
{
    //burası paymenttan fail gelince stock servisine rollback için mesaj yollayacağız o yüzden oluşturduk
    //event yerine mesaj gönderöek istedik yapılacak işin mesajı
    public class StockRollBackRequestMessage : IStockRollBackRequestMessage
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}