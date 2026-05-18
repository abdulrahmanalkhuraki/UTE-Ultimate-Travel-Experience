using Application.DTOs.Country.Response;
using Application.DTOs.Hotel.Response;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public class CountryProfile : Profile
    {
        public CountryProfile()
        {
            // Response mapping
            CreateMap<Country, CountryResponse>();
        }
    }
}
