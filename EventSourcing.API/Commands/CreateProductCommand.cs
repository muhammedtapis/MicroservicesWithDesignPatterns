using EventSourcing.API.DTOs;
using MediatR;

namespace EventSourcing.API.Commands
{
    //commandlar, IRequest MEdiatr. geliyo,bu commandın kullanacağı dto verilmeli.
    //bu commandı controllerda çağırcaz.
    public class CreateProductCommand : IRequest
    {
        public CreateProductDTO? CreateProductDTO { get; set; }
    }
}