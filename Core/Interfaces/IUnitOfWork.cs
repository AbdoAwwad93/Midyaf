using Midyaf.Models;

namespace Midyaf.Core.Interfaces;

public interface IUnitOfWork:IDisposable
{
    IGenericRepository<AppUser>  Users { get; }
    IGenericRepository<Hotel> Hotels  { get; }
    IGenericRepository<Room> Rooms { get; }
    IGenericRepository<Reservation> Reservations { get; }
    IGenericRepository<Review> Reviews { get; }
    Task <int> SaveAsync();
}