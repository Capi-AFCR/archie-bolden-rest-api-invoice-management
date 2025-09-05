using AutoMapper;
using RestAPIInvoiceManagement.Application.DTOs;
using RestAPIInvoiceManagement.Domain.Entities;

namespace RestAPIInvoiceManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Invoice mappings
        CreateMap<Invoice, InvoiceDto>();
        CreateMap<CreateInvoiceDto, Invoice>()
            .ForMember(dest => dest.Payments, opt => opt.Ignore());
        CreateMap<UpdateInvoiceDto, Invoice>()
            .ForMember(dest => dest.Payments, opt => opt.Ignore());

        // Client mappings
        CreateMap<Client, ClientDto>();
        CreateMap<CreateClientDto, Client>();
        CreateMap<UpdateClientDto, Client>();

        // Payment mappings
        CreateMap<Payment, PaymentDto>();
        CreateMap<CreatePaymentDto, Payment>();
        CreateMap<UpdatePaymentDto, Payment>();
    }
}