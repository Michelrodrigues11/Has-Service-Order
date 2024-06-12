using AutoMapper;
using OsDsII.api.Dtos;
using OsDsII.api.Dtos.Customers;
using OsDsII.api.Dtos.ServiceOrders;
using OsDsII.api.Models;

namespace OsDsII.api
{
    public class ProfileMapping : Profile
    {
        public ProfileMapping()
        {
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<CustomerDto, Customer>();
            CreateMap<CommentDto, Comment>();
            CreateMap<ServiceOrderDto, NewServiceOrderDto>();
            CreateMap<NewServiceOrderDto, ServiceOrderDto>();
            CreateMap<NewServiceOrderDto, ServiceOrderDto>();
            CreateMap<ServiceOrder, NewServiceOrderDto>();
            CreateMap<CreateServiceOrderDto, ServiceOrder>();
            CreateMap<NewServiceOrderDto, ServiceOrder>();
        }
    }
}