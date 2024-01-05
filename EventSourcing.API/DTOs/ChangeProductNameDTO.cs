namespace EventSourcing.API.DTOs
{
    public class ChangeProductNameDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}