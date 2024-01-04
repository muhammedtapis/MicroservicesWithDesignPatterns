using MassTransit;
using SharedLibrary.Messages;

namespace SharedLibrary.Interfaces
{
    //artık her bir eventimizde taşımak için correlationId lazım
    //çünkü statemachine bu id ye göre hangi event hangi kuyruk nereye gidicek onu takip edecek vertabanında hangş satırla ilişkili onu bulacak
    public interface IOrderCreatedRequestEvent : CorrelatedBy<Guid>
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}