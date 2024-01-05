namespace EventSourcing.API.DTOs
{
    public class ChangeProductPriceDTO
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
    }
}