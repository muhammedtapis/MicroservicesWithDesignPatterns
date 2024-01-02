using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.DTO;
using Order.API.Model;
using SharedLibrary;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(AppDbContext appDbContext, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDTO orderCreateDTO)
        {
            //KODLA MAPLEME

            var newOrder = new Model.Orderr
            {
                BuyerId = orderCreateDTO.BuyerId,
                Status = OrderStatus.Suspend,
                Address = new Address() { Line = orderCreateDTO.Address.Line, Province = orderCreateDTO.Address.Province, District = orderCreateDTO.Address.District },
                CreatedDate = DateTime.Now,
            };

            //dtodan gelen orderItems listesini tek tek dön herbirine orderitem oluştur ve bilgileri doldur
            orderCreateDTO.OrderItems.ForEach(item =>
            {
                newOrder.Items.Add(new OrderItem() { Price = item.Price, ProductId = item.ProductId, Count = item.Count });
            });

            await _appDbContext.AddAsync(newOrder);
            await _appDbContext.SaveChangesAsync();

            //başarılıysa event oluştur

            var orderCreatedEvent = new OrderCreatedEvent()
            {
                BuyerId = orderCreateDTO.BuyerId,
                OrderrId = newOrder.Id,
                Payment = new PaymentMessage()
                {
                    CardName = orderCreateDTO.Payment.CardName,
                    CardNumber = orderCreateDTO.Payment.CardNumber,
                    CVV = orderCreateDTO.Payment.CVV,
                    Expiration = orderCreateDTO.Payment.Expiration,
                    TotalPrice = orderCreateDTO.OrderItems.Sum(item => item.Price * item.Count), //her itemi adediyle çarparak topla bir üründen 2 yada 3 tane olabilir.
                }
            };

            orderCreateDTO.OrderItems.ForEach(item =>
            {
                orderCreatedEvent.OrderItemMessages.Add(new OrderItemMessage() { Count = item.Count, ProductId = item.ProductId });
            });

            await _publishEndpoint.Publish(orderCreatedEvent);

            //autoMapping ilemapleme yapma çalışıyor şuan ama fiytı falan emin değilim.

            #region CreateWithMapping

            //<-------------------------------------------------------------->
            //var address = _mapper.Map<Address>(orderCreateDTO.Address);

            //var orderItems = _mapper.Map<List<OrderItem>>(orderCreateDTO.OrderItems);

            //var order = _mapper.Map<Orderr>(orderCreateDTO);

            //order.Address = address;
            //order.Items = orderItems;
            //order.CreatedDate = DateTime.Now; //tarihi ver

            //await _appDbContext.AddAsync(order);
            //await _appDbContext.SaveChangesAsync();

            ////işlem başarılıysa event oluştur
            //var payment = _mapper.Map<PaymentMessage>(orderCreateDTO.Payment);

            ////map dosyasında orderdan gelen Id ve ordercreatedeventin OrderrId sini belirtmen gerek yoksa maplemez
            //var orderevent = _mapper.Map<OrderCreatedEvent>(order);

            //payment.TotalPrice = orderItems.Sum(x => x.Price * x.Count); //total price ürünlere göre ekliyo gördük
            //orderevent.Payment = payment;

            //var orderItemMessage = _mapper.Map<List<OrderItemMessage>>(orderItems);

            //orderevent.OrderItemMessages = orderItemMessage;

            //await _publishEndpoint.Publish(orderevent);
            //<-------------------------------------------------------------->

            #endregion CreateWithMapping

            return Ok();
        }
    }
}