using Midyaf.Models;
using Midyaf.Repository;

namespace Midyaf.UnitOfWork;

public interface IUnitOfWork:IDisposable
{
    IGenericRepository<AppUser>  Users { get; }
    IGenericRepository<Property> Propertys  { get; }
    IGenericRepository<Room> Rooms { get; }
    IGenericRepository<Reservation> Reservations { get; }
    IGenericRepository<Review> Reviews { get; }
    IGenericRepository<RoomType> RoomTypes { get; }
    Task <int> SaveAsync();
}