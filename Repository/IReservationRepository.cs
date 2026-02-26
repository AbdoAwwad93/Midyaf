using Midyaf.Models;

namespace Midyaf.Repository;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<List<Reservation>> GetByUserIdAsync(string userId);
}
