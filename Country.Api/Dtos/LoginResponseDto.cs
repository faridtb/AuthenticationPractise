using Country.Api.Entities;

namespace Country.Api.Dtos
{
    public class LoginResponseDto
    {
        public string? Message { get; set; }
        public bool? IsSuccess { get; set; }
        public string? Token { get; set; }
        public User? User { get; set; }

    }
}
