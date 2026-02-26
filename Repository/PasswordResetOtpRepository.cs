using Microsoft.EntityFrameworkCore;
using Midyaf.Data;
using Midyaf.Models;

namespace Midyaf.Repository;

public class PasswordResetOtpRepository : GenericRepository<PasswordResetOtp>, IPasswordResetOtpRepository
{
    public PasswordResetOtpRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<List<PasswordResetOtp>> GetActiveOtpsAsync(string userId)
    {
        return await _dbset
            .Where(o => o.UserId == userId && !o.IsUsed)
            .ToListAsync();
    }

    public async Task<PasswordResetOtp?> GetValidOtpAsync(string userId, string otp, DateTime utcNow)
    {
        return await _dbset
            .Where(o => o.UserId == userId &&
                        o.Otp == otp &&
                        !o.IsUsed &&
                        o.ExpiresAt > utcNow)
            .FirstOrDefaultAsync();
    }
}
