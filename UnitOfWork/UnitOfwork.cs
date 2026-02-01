using Midyaf.Data;
using Midyaf.Models;
using Midyaf.Repository;
namespace Midyaf.UnitOfWork;

public class UnitOfwork:IUnitOfWork
{
    private readonly AppDbContext _context;
    public IGenericRepository<AppUser> Users { get; }
    public IGenericRepository<Hotel> Hotels { get; }
    public IGenericRepository<Room> Rooms { get; }
    public IGenericRepository<Reservation> Reservations { get; }
    public IGenericRepository<Review> Reviews { get; }
    public IGenericRepository<RoomType> RoomTypes { get; }

    public UnitOfwork(AppDbContext context)
    {
        _context = context;
        Users = new GenericRepository<AppUser>(_context);
        Hotels = new GenericRepository<Hotel>(_context);
        Rooms = new GenericRepository<Room>(_context);
        Reservations = new GenericRepository<Reservation>(_context);
        Reviews = new GenericRepository<Review>(_context);
        RoomTypes = new GenericRepository<RoomType>(_context);
        
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