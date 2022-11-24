using Country.Api.Data;
using Country.Api.Dtos;
using Country.Api.Entities;
using Country.Api.Interfaces.Repository;
using Country.Api.Interfaces.Services;
using Country.Api.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Security.Cryptography;
using System.Text;

namespace Country.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IIdentityRepository identityRepository;
        private readonly ITokenService tokenService;
        private readonly IEmailService emailService;

        public AccountController(IIdentityRepository identityRepository, 
            ITokenService tokenService, IEmailService emailService)
        {
            this.identityRepository = identityRepository;
            this.tokenService = tokenService;
            this.emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromQuery] UserCreateDto userCreateDto)
        {
            // Releasede achmaq sureti ile
            //UserCreateValidator validator = new();
            //var validResult = validator.Validate(userCreateDto);

            //if (!validResult.IsValid) return BadRequest(validResult.Errors);

            if (userCreateDto.Password != userCreateDto.RepeatePassword) 
                return BadRequest("Passwords didn't match");

            using var hmac = new HMACSHA512();
            User newUser = new()
            {
                Name = userCreateDto.Name,
                Surname = userCreateDto.Surname,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userCreateDto.Password)),
                PasswordSalt = hmac.Key,
                Email = userCreateDto.Email,
                Username = userCreateDto.Username,
                Phone = userCreateDto.Phone,
                Gender = userCreateDto.Gender,
                LastModifiedDate = DateTime.Now,
                IsActive = false,
                ActivationPass = RandomString()
            };

            GeneralResponse response = await identityRepository.AddAsync(newUser);

            if (response.IsSucceed == false)
                return BadRequest(response.Message);
            

            bool send = emailService.SendEmail(new MailBodyDto {
                Email = userCreateDto.Email,
                Content = "<!DOCTYPE html>" +
                                     "<html> " +
                                     "<body style=\"background -color:#ff7f26;text-align:center;\"> " +
                                     "<h3 style=\"color:#051a80;\">Find Your Region App</h1> " +
                                     "<h3 style=\"color:rebeccapurple;\">Please scan the qrcode below and activate your account with the password you received. </h3>" +
                                     "</body> " +
                                     "</html>",
                Subject = userCreateDto.Name,
                Code = QrCodeGenerator(newUser.ActivationPass),
                
            });

            if (!send)
                return BadRequest("Email can't send to adress :(");

            return Ok($"User Created,\nActivation Code sended via QR Code to email:{newUser.Email}\nUser ID:{newUser.Id}");
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto login)
        {
            LoginResponseDto response = await identityRepository.Login(login);

            if (response.IsSuccess == false) return BadRequest(response.Message);

            if (response.User.IsActive == false) return BadRequest($"First you need to activate your account\n" +
                $"User Id:{response.User.Id}");

            return Ok(new LoginResponseDto
            {
                Message = response.User.Username,
                Token = tokenService.CreateToken(response.User),
            });
        }

        [HttpPost("activation")]
        public async Task<ActionResult> Activate(Guid userId, string activationCode)
        {
            bool activated = await identityRepository.Activation(userId, activationCode);

            return activated == true ? Ok($"User successfully activated") : BadRequest("Activation failed");
        }

        [HttpGet("users")]
        public async Task<List<User>> GetAll()
        {
            return await identityRepository.GetAllAsync();
        }

        [HttpGet("users/{id}")]
        public async Task<User> GetOne(Guid id)
        {
            return await identityRepository.GetByIdAsync(id);
        }

        [HttpDelete("users/{id}")]
        public async Task<GeneralResponse> Delete(Guid id)
        {
            return await identityRepository.RemoveAsync(id);
        }

        [HttpPut("user/update/{id}")]
        public async Task<GeneralResponse> Update(Guid id, UserUpdateDto update)
        {
            User dbUser = await identityRepository.GetByIdAsync(id);

            if (dbUser is null)
                return new GeneralResponse { IsSucceed = false, Message = "User didn't exist" };

            using var hmac = new HMACSHA512();

            dbUser.Email = update.Email;
            dbUser.Username = update.Username;
            dbUser.Name = update.Name ?? dbUser.Name;
            dbUser.Surname = update.Surname ?? dbUser.Surname;
            dbUser.Gender = update.Gender ?? dbUser.Gender;
            dbUser.PasswordHash = update.Password != null ? hmac.ComputeHash(Encoding.UTF8.GetBytes(update.Password)) : dbUser.PasswordHash;
            dbUser.PasswordSalt = update.Password != null ? hmac.Key : dbUser.PasswordHash;
            dbUser.LastModifiedDate = DateTime.Now;

            return await identityRepository.UpdateAsync(dbUser);
        }

        private string QrCodeGenerator(string password)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(password, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            var bitmap = qrCode.GetGraphic(20);
            string code = "data:image/png;base64," + Convert.ToBase64String(bitmap.ToArray());
            return code;
        }

        private string RandomString(int size = 6, bool lowerCase = true)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

    }
}
