using Country.Api.Entities;

namespace Country.Api.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
