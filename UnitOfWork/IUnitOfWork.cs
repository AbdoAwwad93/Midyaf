using Midyaf.Models;
using Midyaf.Repository;

namespace Midyaf.UnitOfWork;

public interface IUnitOfWork:IDisposable
{
    IGenericRepository<AppUser>  Users { get; }
    IPropertyRepository Properties  { get; }
    IGenericRepository<Room> Rooms { get; }
    IReservationRepository Reservations { get; }
    IGenericRepository<Review> Reviews { get; }
    IGenericRepository<RoomType> RoomTypes { get; }
    IPasswordResetOtpRepository PasswordResetOtps { get; }
    Task <int> SaveAsync();
}