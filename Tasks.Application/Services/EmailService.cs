using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tasks.Core.Entities.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Tasks.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string recipient, string subject, string body)
        {
            // Read email settings from appsettings.json
            var emailSettings = _configuration.GetSection("EmailSettings");
            var defaultFromEmail = _configuration["DefaultFromEmail"];

            // Configure SMTP client
            var smtpClient = new SmtpClient(emailSettings["Host"], int.Parse(emailSettings["Port"]))
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
                EnableSsl = bool.Parse(emailSettings["UseTLS"])
            };

            // Create and configure the email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(defaultFromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(recipient);

            // Send the email
            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
