using Ordering.Application.Models;

namespace Ordering.Infrastructure.Helper.Mail
{
    public interface IEmailService
    {
        Task<bool> SendEmail(Email email);
    }
}
