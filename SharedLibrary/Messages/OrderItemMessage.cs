namespace SharedLibrary.Messages
{
    public class OrderItemMessage
    {
        public int ProductId { get; set; } //hangi producta ait bu order item
        public int Count { get; set; } //stock.API ilgili ProductID sahip productttan kaç adet azaltacak gerekli
    }
}