using Application.DTOs.Payment.Response;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, PaymentResponse>();
    }
}
