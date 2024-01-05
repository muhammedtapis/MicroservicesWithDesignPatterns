using EventSourcing.API.Commands;
using EventSourcing.API.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace EventSourcing.API.Controllers
{
    //handlerlarımızı burada kullanacağız.

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator; //MediatR kütüphanesinden geliyo bu interface üzerinden commandları commandhandlera gönderiyoruz.

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDTO createProductDTO)
        {
            //burada yaptığımız şey ; Gelen DTO commande çevriliyor commanden evente çevriliyor sonra eventstore kaydediliyor.
            await _mediator.Send(new CreateProductCommand() { CreateProductDTO = createProductDTO });

            //ÖNEMLİ NOKTA buradaki send metodu bu Commandı işleyen yani handle eden yere gönderiyor!!! CommandHandler bunu handle ediyor.

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangeName(ChangeProductNameDTO changeProductNameDTO)
        {
            await _mediator.Send(new ChangeProductNameCommand() { ChangeProductNameDTO = changeProductNameDTO });

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangePrice(ChangeProductPriceDTO changeProductPriceDTO)
        {
            await _mediator.Send(new ChangeProductPriceCommand() { ChangeProductPriceDTO = changeProductPriceDTO });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteProductCommand() { Id = id });
            return NoContent();
        }
    }
}