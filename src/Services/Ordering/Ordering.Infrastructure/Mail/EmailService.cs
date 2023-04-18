using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Models;
using Ordering.Infrastructure.Helper.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Mail
{
    public class EmailService : IEmailService
    {
        public EmailSettings _emailSettings { get; }
        public ILogger<EmailService> _logger { get; }

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value ?? throw new ArgumentNullException(nameof(emailSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SendEmail(Email email)
        {
            var client = new SendGridClient(_emailSettings.ApiKey);
            var sendGridMessage = MailHelper.CreateSingleEmail(
                new EmailAddress
                {
                    Email = _emailSettings.FromAddress,
                    Name = _emailSettings.FromName
                },
                new EmailAddress
                {
                    Email = email.To
                },
                email.Subject,
                email.Body,
                email.Body
            );
            var response = await client.SendEmailAsync(sendGridMessage);

            _logger.LogInformation("Email sent.");
            if (response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.OK)
                return true;

            _logger.LogError("Email sending failed.");
            return false;
        }
    }
}
