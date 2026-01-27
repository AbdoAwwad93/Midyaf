using AutoMapper;
using Midyaf.Models.DTOs;
using Midyaf.Models;

namespace Midyaf.Models.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Hotel,HotelDTO>().ReverseMap();
            CreateMap<AppUser, RegisterDTO>().ReverseMap();
            CreateMap<Room, RoomDTO>().ReverseMap();
            CreateMap<Reservation, ReservationDTO>().ReverseMap();
        }
    }
}
