using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class RabbitMQSettingsConst
    {
        //stock apide ORderCreatedEvent bekleyen kuyruğumuzun adı.
        public const string StockOrderCreatedEventQueueName = "stock-order-created-queue"; //stock için, orderCreatedevent dinleyeceği kuyruk

        //payment apide stockreservedevent bekleyen kuyruk
        public const string StockReservedEventQueueName = "stock-reserved-queue"; //stoğun kendi fıralttığı eventin girdiği kuyruk

        public const string PaymentStockReservedEventQueueName = "payment-stock-reserved-queue"; //payment için subscribe olduğumuz kuyruk ismi
    }
}