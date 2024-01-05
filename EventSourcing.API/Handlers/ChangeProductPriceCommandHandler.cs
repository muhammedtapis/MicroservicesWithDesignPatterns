using EventSourcing.API.Commands;
using EventSourcing.API.EventStores;
using MediatR;

namespace EventSourcing.API.Handlers
{
    public class ChangeProductPriceCommandHandler : IRequestHandler<ChangeProductPriceCommand>
    {
        private readonly ProductStream _productStream;

        public ChangeProductPriceCommandHandler(ProductStream productStream)
        {
            _productStream = productStream;
        }

        public async Task<Unit> Handle(ChangeProductPriceCommand request, CancellationToken cancellationToken)
        {
            if (request.ChangeProductPriceDTO != null)
            {
                _productStream.PriceChanged(request.ChangeProductPriceDTO);
                await _productStream.SaveAsync();
            }
            else
            {
                throw new Exception("ChangeProductPriceDTO boş");
            }

            return Unit.Value;
        }
    }
}