using Microsoft.EntityFrameworkCore;
using Midyaf.Data;
using Midyaf.Models;

namespace Midyaf.Repository;

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<List<Reservation>> GetByUserIdAsync(string userId)
    {
        return await _dbset
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }
}
