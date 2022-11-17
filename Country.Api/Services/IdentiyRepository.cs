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

        public async Task<GeneralResponse> AddAsync(User entity)
        {
            try
            {
                GeneralResponse response = await CheckUserAsync(entity.Email, entity.Username);
                if (response.IsSucceed == true)
                {
                    await _context.Users.AddAsync(entity);
                    await _context.SaveChangesAsync();
                }
                return response;
            }
            catch (Exception)
            {
                return new GeneralResponse { IsSucceed = false, Message = "Server Error"};
            }
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
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Username == logindto.UsernameOrEmail || u.Email == logindto.UsernameOrEmail);
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
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            try
            {
                User dbUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                return dbUser;
            }
            catch (NullReferenceException)
            {  
                throw new Exception("User didn't exist in our database");
            }
        }

        public async Task<GeneralResponse> RemoveAsync(Guid id)
        {
            try
            {
                User dbUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                _context.Users.Remove(dbUser);
                await _context.SaveChangesAsync();
                return new GeneralResponse { IsSucceed = true, Message = "User succesfully deleted from database"};
            }
            catch (NullReferenceException)
            {
                throw new Exception("User didn't exist in our database");
            }
        }

        public async Task<GeneralResponse> UpdateAsync(User user)
        {
            try
            {
                User dbUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

                if (user.Email is not null)
                {
                    bool checkEmail = await CheckAsEmailAsync(user.Email);
                    if (checkEmail)
                        return new GeneralResponse { IsSucceed = false, Message = "Email already taken" };
                }

                if (user.Username is not null)
                {
                    bool chekUsername = await CheckAsUsernameAsync(user.Username);
                    if (chekUsername)
                        return new GeneralResponse { IsSucceed = false, Message = "Username already taken" } ;
                }

                #region Update
                dbUser.Username = user.Username ?? dbUser.Username;
                dbUser.Email = user.Email ?? dbUser.Email;
                #endregion

                await _context.SaveChangesAsync();
                return new GeneralResponse { IsSucceed = true, Message = "User succesfully updated" };
            }
            catch (NullReferenceException)
            {
                throw new Exception("User didn't exist in our database");
            }
        }


        private async Task<bool> CheckAsUsernameAsync(string username)
        {
            bool usernameExist = await _context.Users.AnyAsync(x => x.Username == username);
            return usernameExist;
        }
        private async Task<bool>CheckAsEmailAsync(string email)
        {
            bool emailExist = await _context.Users.AnyAsync(x => x.Email == email);
            return emailExist;
        }
        private async Task<GeneralResponse> CheckUserAsync(string email, string username) 
        {
            bool emailExist = await CheckAsEmailAsync(email);
            if (emailExist)
                return new GeneralResponse { IsSucceed = false, Message = "Email already registered" };

            bool usernameExist = await CheckAsUsernameAsync(username);
            if (usernameExist)
                return new GeneralResponse { IsSucceed = false, Message = "Username already taken" };

            return new GeneralResponse { IsSucceed = true };
        }
    }
}
