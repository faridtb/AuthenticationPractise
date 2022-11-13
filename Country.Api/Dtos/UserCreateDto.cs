using Country.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace Country.Api.Dtos
{
    public class UserCreateDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RepeatePassword { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
    }
}
