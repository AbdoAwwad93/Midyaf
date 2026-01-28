using System.Net;
using System.Net.Mail;
using Midyaf.Models;
using Midyaf.Services.Interfaces;

namespace Midyaf.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService()
    {
        _smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
        _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        _smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "";
        _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
        _fromEmail = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL") ?? "";
        _fromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME") ?? "Midyaf";
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SendBookingConfirmationAsync(string to, Reservation reservation)
    {
        var subject = $"Booking Confirmation - #{reservation.Id}";
        var body = GetBookingConfirmationTemplate(reservation);
        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendBookingCancellationAsync(string to, Reservation reservation)
    {
        var subject = $"Booking Cancelled - #{reservation.Id}";
        var body = GetBookingCancellationTemplate(reservation);
        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendOtpAsync(string to, string otp)
    {
        var subject = "Password Reset OTP - Midyaf";
        var body = GetOtpTemplate(otp);
        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendPaymentReceiptAsync(string to, Reservation reservation, decimal amount)
    {
        var subject = $"Payment Receipt - Booking #{reservation.Id}";
        var body = GetPaymentReceiptTemplate(reservation, amount);
        return await SendEmailAsync(to, subject, body);
    }

    private string GetBookingConfirmationTemplate(Reservation reservation)
    {
        var roomsList = reservation.Rooms != null 
            ? string.Join(", ", reservation.Rooms.Select(r => r.RoomNumber))
            : "N/A";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #2563eb; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9fafb; }}
        .details {{ background: white; padding: 15px; border-radius: 8px; margin: 15px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Booking Confirmed!</h1>
        </div>
        <div class='content'>
            <p>Your booking has been confirmed. Here are your reservation details:</p>
            <div class='details'>
                <p><strong>Booking ID:</strong> #{reservation.Id}</p>
                <p><strong>Check-in:</strong> {reservation.CheckIn:dddd, MMMM dd, yyyy}</p>
                <p><strong>Check-out:</strong> {reservation.CheckOut:dddd, MMMM dd, yyyy}</p>
                <p><strong>Number of Guests:</strong> {reservation.NumberOfGuests}</p>
                <p><strong>Room(s):</strong> {roomsList}</p>
            </div>
            <p>Thank you for choosing Midyaf!</p>
        </div>
        <div class='footer'>
            <p>&copy; {DateTime.Now.Year} Midyaf. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GetBookingCancellationTemplate(Reservation reservation)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #dc2626; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9fafb; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Booking Cancelled</h1>
        </div>
        <div class='content'>
            <p>Your booking #{reservation.Id} has been cancelled.</p>
            <p><strong>Original Check-in:</strong> {reservation.CheckIn:dddd, MMMM dd, yyyy}</p>
            <p><strong>Original Check-out:</strong> {reservation.CheckOut:dddd, MMMM dd, yyyy}</p>
            <p>If you have any questions, please contact our support team.</p>
        </div>
        <div class='footer'>
            <p>&copy; {DateTime.Now.Year} Midyaf. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GetOtpTemplate(string otp)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #2563eb; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9fafb; text-align: center; }}
        .otp-box {{ background: white; padding: 30px; border-radius: 8px; margin: 20px 0; }}
        .otp-code {{ font-size: 36px; font-weight: bold; letter-spacing: 8px; color: #2563eb; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Password Reset</h1>
        </div>
        <div class='content'>
            <p>We received a request to reset your password.</p>
            <p>Use the following OTP code to reset your password:</p>
            <div class='otp-box'>
                <span class='otp-code'>{otp}</span>
            </div>
            <p>This code will expire in <strong>10 minutes</strong>.</p>
            <p><small>If you didn't request this, please ignore this email.</small></p>
        </div>
        <div class='footer'>
            <p>&copy; {DateTime.Now.Year} Midyaf. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GetPaymentReceiptTemplate(Reservation reservation, decimal amount)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #059669; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9fafb; }}
        .amount {{ font-size: 24px; font-weight: bold; color: #059669; text-align: center; padding: 20px; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Payment Receipt</h1>
        </div>
        <div class='content'>
            <p>Thank you for your payment!</p>
            <div class='amount'>${amount:F2}</div>
            <p><strong>Booking ID:</strong> #{reservation.Id}</p>
            <p><strong>Check-in:</strong> {reservation.CheckIn:dddd, MMMM dd, yyyy}</p>
            <p><strong>Check-out:</strong> {reservation.CheckOut:dddd, MMMM dd, yyyy}</p>
            <p><strong>Payment Date:</strong> {DateTime.Now:dddd, MMMM dd, yyyy}</p>
        </div>
        <div class='footer'>
            <p>&copy; {DateTime.Now.Year} Midyaf. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }
}
