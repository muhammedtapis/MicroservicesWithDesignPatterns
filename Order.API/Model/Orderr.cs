namespace Order.API.Model
{
    public class Orderr
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } //oluşturulma tarihi

        public string BuyerId { get; set; } //kim oluşturdu bu siparişi

        public Address Address { get; set; } //owned entity type tanımladık burda artık addres tablosu order tablosunda gözükecek
        public OrderStatus Status { get; set; } //sipariş durumu

        //eğer status fail ise mesaj göstericez
        public string FailMessage { get; set; }

        //navigation prop. bir orderda birden fazla orderitem olabilir mesela tişört,pant,gömlek
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    //sipariş durumunu tutacağımız enum
    public enum OrderStatus
    {
        Suspend,
        Succeed,
        Fail
    }
}