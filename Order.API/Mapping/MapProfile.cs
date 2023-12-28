using AutoMapper;
using Order.API.DTO;
using Order.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()

        {
            CreateMap<OrderCreateDTO, OrderCreatedEvent>().ReverseMap();
            CreateMap<OrderCreateDTO.AddressDTO, Address>().ReverseMap();
            CreateMap<OrderCreateDTO.OrderItemDTO, OrderItem>().ReverseMap();
            CreateMap<OrderCreateDTO.PaymentDTO, PaymentMessage>().ReverseMap();
            CreateMap<OrderCreateDTO, Orderr>().ReverseMap();
            CreateMap<OrderItem, OrderItemMessage>().ReverseMap();
            CreateMap<Orderr, OrderCreatedEvent>().ReverseMap();
        }
    }
}