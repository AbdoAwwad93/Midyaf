using Midyaf.Models;

namespace Midyaf.Repository;

public interface IPasswordResetOtpRepository : IGenericRepository<PasswordResetOtp>
{
    Task<List<PasswordResetOtp>> GetActiveOtpsAsync(string userId);
    Task<PasswordResetOtp?> GetValidOtpAsync(string userId, string otp, DateTime utcNow);
}
