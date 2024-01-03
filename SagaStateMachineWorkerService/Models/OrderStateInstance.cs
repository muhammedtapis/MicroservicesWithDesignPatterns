using MassTransit;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SagaStateMachineWorkerService.Models
{
    public class OrderStateInstance : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        //buraya denk gelen stateler ; Initial,OrderCreated,PaymentFailed,StockReserved,Final... bunlar event değil state
        //initial ve final kütüphaneden hazır geliyor diğerlerini biz kendimiz oluşturucağız
        public string CurrentState { get; set; }

        public string BuyerId { get; set; }
        public int OrderId { get; set; } //orderState event göndericez ilgili sipariş için

        //kart bilgileri
        public string CardName { get; set; }

        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        //orderstate veritabanında tutulurken oluşturulma tarihi
        public DateTime CreatedDate { get; set; }

        //orderstateinstance yazdırmak için override ettik
        public override string ToString()
        {
            var properties = GetType().GetProperties(); //bu sınıftaki prop ları al

            StringBuilder sb = new StringBuilder();

            properties.ToList().ForEach(p =>
            {
                var value = p.GetValue(this, null); //bu propun valuesını al
                sb.AppendLine($"{p.Name}:{value}");
            });

            sb.Append("--------------------");
            return sb.ToString();
        }
    }
}