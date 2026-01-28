using Midyaf.Models;

namespace Midyaf.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task<bool> SendBookingConfirmationAsync(string to, Reservation reservation);
    Task<bool> SendBookingCancellationAsync(string to, Reservation reservation);
    Task<bool> SendOtpAsync(string to, string otp);
    Task<bool> SendPaymentReceiptAsync(string to, Reservation reservation, decimal amount);
}
