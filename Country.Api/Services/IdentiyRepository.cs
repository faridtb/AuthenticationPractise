using Country.Api.Data;
using Country.Api.Dtos;
using Country.Api.Entities;
using Country.Api.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Country.Api.Services
{
    public class IdentiyRepository : IIdentityRepository
    {
        private readonly AppDbContext _context;

        public IdentiyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(User entity)
        {
            try
            {
                User existUser = await _context.Users.FirstOrDefaultAsync(x =>x.Username.ToLower() == entity.Username.ToLower());
                if (existUser != null) return false;
                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> Activation(Guid userId, string activationCode)
        {
            try
            {
                User user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user is null) return false;

                if (user.ActivationPass == activationCode)
                {
                    user.IsActive = true;
                    await _context.SaveChangesAsync();

                    return true;
                }
                return false;             
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<LoginResponseDto> Login(LoginDto logindto)
        {
            try
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Username == logindto.Username || u.Email == logindto.Email);
                if (user is null)
                    return new LoginResponseDto { IsSuccess = false,Message = "Username or Email doesnt exist"};

                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.Password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i]) 
                        return new LoginResponseDto { IsSuccess = false, Message = "Password was wrong"};
                }

                return new LoginResponseDto { IsSuccess = true, Message = "Login successful", User = user };
            }
            catch (Exception)
            {
                throw ;
            }
        }


        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public Task<User> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Guid id)
        {
            throw new NotImplementedException();
        }


    }
}
