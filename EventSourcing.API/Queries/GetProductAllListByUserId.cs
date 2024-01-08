using EventSourcing.API.DTOs;
using MediatR;

namespace EventSourcing.API.Queries
{
    //commandlerin aksine IRequestten kalıtım alırken döneceği değeri de veriyoruz.
    //yine bunun için handler oluşturcaksın.
    public class GetProductAllListByUserId : IRequest<List<ProductDTO>>
    {
        public int UserId { get; set; }
    }
}