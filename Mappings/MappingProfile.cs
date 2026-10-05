using AutoMapper;
using Phase_07_Poc_01.ApiModels;
using Phase_07_Poc_01.DTO.CartDtos;
using Phase_07_Poc_01.DTO.OrderDtos;
using Phase_07_Poc_01.DTO.ProductDtos;
using Phase_07_Poc_01.DTO.UserDtos;
using Phase_07_Poc_01.Infrastructure.Entities;
using UserProductCart.WebApi.ApiModels;

namespace Phase_07_Poc_01.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DTO.ProductDtos.ProductDto, Product>().ReverseMap();
            CreateMap<DTO.ProductDtos.CreateProductDto, Product>().ReverseMap();
            CreateMap<DTO.UserDtos.UserDto, User>().ReverseMap();

            CreateMap<UserDto, UserApiModel>();
            CreateMap<ProductDto, ProductApiModel>();
            
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Product.Price * src.Quantity));

            CreateMap<CartItemDto, CartItemApiModel>();

            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<OrderDto, OrderApiModel>();
            CreateMap<OrderItemDto, OrderItemApiModel>();
        }
    }
}