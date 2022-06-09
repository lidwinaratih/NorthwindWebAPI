using AutoMapper;
using Northwind.Entities.Models;
using Northwind.Entities.DataTransferObject;

namespace NorthwindWebApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Get Cateegory : ngambil data dari tabel Category dimasukin kedalam Dto
            CreateMap<Category, CategoryDto>().ReverseMap();
            // Post => inputan dari Dto (dalam bentuk Dto) dimasukkan ke dalam tabel Category
            //CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryUpdateDto>().ReverseMap();

            CreateMap<Customer, CustomerDto>().ReverseMap();
            //CreateMap<CustomerDto, Customer>();
            CreateMap<Customer, CustomerUpdateDto>().ReverseMap();
            
            CreateMap<UserForRegistrationDto, User>();

            CreateMap<Order, OrdersDto>();
            CreateMap<OrdersDto, Order>();

            CreateMap<OrderDetail, OrderDetailDto>();
            CreateMap<OrderDetailDto, OrderDetail>();

            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
        }
    }
}
