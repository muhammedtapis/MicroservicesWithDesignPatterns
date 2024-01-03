using AutoMapper;
using Order.API.DTO;
using Order.API.Model;
using SharedLibrary.Events;

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

            //maplerken property isimleri aynı olmazsa bu şekilde tek tek belirtmen lazım yoksa maplemiyor. birinde OrderrId diğerinde Id çünkü
            //CreateMap<Orderr, OrderCreatedEvent>().ForMember(x => x.OrderrId, opt => opt.MapFrom(y => y.Id));
        }
    }
}