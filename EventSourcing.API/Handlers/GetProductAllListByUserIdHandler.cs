using EventSourcing.API.DTOs;
using EventSourcing.API.Models;
using EventSourcing.API.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.API.Handlers
{
    //datayı artık sql den okuycaz eventstoredan deil
    public class GetProductAllListByUserIdHandler : IRequestHandler<GetProductAllListByUserId, List<ProductDTO>>
    {
        private readonly AppDbContext _context;

        public GetProductAllListByUserIdHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDTO>> Handle(GetProductAllListByUserId request, CancellationToken cancellationToken)
        {
            var products = await _context.Products.Where(x => x.UserId == request.UserId).ToListAsync();

            return products.Select(x => new ProductDTO { Id = x.Id, Name = x.Name, Price = x.Price, Stock = x.Stock, UserId = x.UserId }).ToList();
        }
    }
}