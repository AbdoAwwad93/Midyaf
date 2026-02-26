using Midyaf.Data;
using Midyaf.Models;
using Midyaf.Repository;
namespace Midyaf.UnitOfWork;

public class UnitOfwork:IUnitOfWork
{
    private readonly AppDbContext _context;
    public IGenericRepository<AppUser> Users { get; }
    public IPropertyRepository Propertys { get; }
    public IGenericRepository<Room> Rooms { get; }
    public IReservationRepository Reservations { get; }
    public IGenericRepository<Review> Reviews { get; }
    public IGenericRepository<RoomType> RoomTypes { get; }
    public IPasswordResetOtpRepository PasswordResetOtps { get; }

    public UnitOfwork(AppDbContext context)
    {
        _context = context;
        Users = new GenericRepository<AppUser>(_context);
        Propertys = new PropertyRepository(_context);
        Rooms = new GenericRepository<Room>(_context);
        Reservations = new ReservationRepository(_context);
        Reviews = new GenericRepository<Review>(_context);
        RoomTypes = new GenericRepository<RoomType>(_context);
        PasswordResetOtps = new PasswordResetOtpRepository(_context);
        
    }
    
    public async Task<int> SaveAsync()
    { 
       return await _context.SaveChangesAsync();
    }
    public void Dispose()
    { 
        _context.Dispose();
    }
}