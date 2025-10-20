using AutoMapper;
using Midyaf.Core.DTOs;
using Midyaf.DTOs;
using Midyaf.Models;

namespace Midyaf.Infrastructure.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Hotel,HotelDTO>().ReverseMap();
            CreateMap<AppUser, RegisterDTO>().ReverseMap();
        }
    }
}
