using AutoMapper;
using NETMockServer.DTOs;
using NETMockServer.Entities;

namespace NETMockServer.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.ProductTags.Select(pt => pt.Tag.Name)));

        CreateMap<ProductCreateDto, Product>()
            .ForMember(d => d.ProductTags, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Customer, CustomerDto>();
        CreateMap<CustomerCreateDto, Customer>();

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => $"{s.Customer.FirstName} {s.Customer.LastName}"))
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name));

        CreateMap<OrderCreateDto, Order>();
        CreateMap<OrderItemCreateDto, OrderItem>()
            .ForMember(d => d.UnitPrice, opt => opt.Ignore())
            .ForMember(d => d.Id, opt => opt.Ignore());
    }
}