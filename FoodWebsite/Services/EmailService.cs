using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using FoodWebsite.Models;

namespace FoodWebsite.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOrderConfirmationEmail(string toEmail, Order order)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Food Website", _config["EmailSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Sipariş Onayı";
            message.Body = new TextPart("html")
            {
                Text = $"<h3>Sipariş Onaylandı!</h3><p>Sipariş #{order.Id} ({order.OrderDate}) {order.TotalAmount:C} tutarında onaylandı.</p>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderPassword"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}