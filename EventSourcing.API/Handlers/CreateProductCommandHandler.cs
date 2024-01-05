using EventSourcing.API.Commands;
using EventSourcing.API.EventStores;
using MediatR;

namespace EventSourcing.API.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
    {
        //request ile command içindeki dataya erişebiliriz.
        private readonly ProductStream _productStream;

        public CreateProductCommandHandler(ProductStream productStream)
        {
            _productStream = productStream;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (request.CreateProductDTO != null)
            {
                _productStream.Created(request.CreateProductDTO!);
                await _productStream.SaveAsync();
            }
            else
            {
                throw new Exception("CreateProductDTO boş");
            }

            return Unit.Value;
        }
    }
}