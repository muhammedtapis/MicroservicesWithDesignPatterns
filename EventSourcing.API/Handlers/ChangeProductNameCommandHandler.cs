using EventSourcing.API.Commands;
using EventSourcing.API.EventStores;
using MediatR;

namespace EventSourcing.API.Handlers
{
    public class ChangeProductNameCommandHandler : IRequestHandler<ChangeProductNameCommand>
    {
        private readonly ProductStream _productStream;

        public ChangeProductNameCommandHandler(ProductStream productStream)
        {
            _productStream = productStream;
        }

        public async Task<Unit> Handle(ChangeProductNameCommand request, CancellationToken cancellationToken)
        {
            if (request.ChangeProductNameDTO != null)
            {
                _productStream.NameChanged(request.ChangeProductNameDTO);
                await _productStream.SaveAsync();
            }
            else
            {
                throw new Exception("ChangeProductNameDTO boş");
            }

            return Unit.Value;
        }
    }
}