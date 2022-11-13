using Country.Api.Dtos;

namespace Country.Api.Interfaces.Services
{
    public interface IEmailService
    {
        bool SendEmail(MailBodyDto body);
    }
}
