namespace Order.API.DTO
{
    public class OrderCreateDTO
    {
        //sipariş oluştururken nelere ihtiyac var ?
        public string BuyerId { get; set; } //hangi kullanıcıya ait

        public List<OrderItemDTO> OrderItems { get; set; }
        public PaymentDTO Payment { get; set; }
        public AddressDTO Address { get; set; }

        public class OrderItemDTO
        {
            public int ProductId { get; set; } //hangi producta ait bu order item
            public int Count { get; set; } //stock.API ilgili productttan kaç adet azaltacak gerekli
            public decimal Price { get; set; }
        }

        public class PaymentDTO
        {
            public string CardName { get; set; }
            public string CardNumber { get; set; }
            public string Expiration { get; set; }
            public string CVV { get; set; }
            public decimal TotalPrice { get; set; }
        }

        public class AddressDTO
        {
            public string Line { get; set; }
            public string Province { get; set; }
            public string District { get; set; } //order tablosunda Address.District diye gözükecek.
        }
    }
}