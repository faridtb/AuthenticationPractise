using Country.Api.Dtos;
using Country.Api.Entities;

namespace Country.Api.Interfaces.Repository
{
    public interface IIdentityRepository : IGenericRepositoryAsync<User>
    {
        Task<LoginResponseDto> Login(LoginDto logindto);
        Task<bool> Activation(Guid userId, string activationCode);
    }
}
