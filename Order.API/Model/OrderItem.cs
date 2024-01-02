using System.ComponentModel.DataAnnotations.Schema;

namespace Order.API.Model
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Count { get; set; } //bu üründen kaç tane satın aldı

        //navigation prop her bir orderitem bir ordera ait
        public int OrderrId { get; set; } //orderid kime ait foreign key

        public Orderr Orderr { get; set; }
    }
}