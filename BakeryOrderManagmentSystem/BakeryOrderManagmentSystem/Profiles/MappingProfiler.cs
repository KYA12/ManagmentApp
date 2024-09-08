using AutoMapper;
using BakeryOrderManagmentSystem.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map Order to OrderDto
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OrdersProducts, opt => opt.MapFrom(src => src.OrdersProducts));

        // Map OrderDto to Order
        CreateMap<OrderDto, Order>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<OrderStatus>(src.Status)))
            .ForMember(dest => dest.OrdersProducts, opt => opt.Ignore());

        // Map OrdersProducts to OrderProductDto and vice versa
        CreateMap<OrdersProducts, OrderProductDto>().ForMember(dest => dest.ProductName, opt => opt.Ignore());
        CreateMap<OrderProductDto, OrdersProducts>();

        // Map Product to ProductDto and vice versa
        CreateMap<Product, ProductDto>();
        CreateMap<ProductDto, Product>();
    }
}