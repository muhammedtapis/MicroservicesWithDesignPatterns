using EventSourcing.API.DTOs;
using MediatR;

namespace EventSourcing.API.Commands
{
    public class ChangeProductPriceCommand : IRequest
    {
        public ChangeProductPriceDTO? ChangeProductPriceDTO { get; set; }
    }
}