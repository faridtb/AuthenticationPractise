using Country.Api.Dtos;
using FluentValidation;

namespace Country.Api.Validators
{
    public class UserCreateValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateValidator()
        {
            RuleFor(u => u.Username).NotEmpty().WithMessage("Can't be empty")
                .MinimumLength(8).WithMessage("Your username length must be at least 5.")
                   .MaximumLength(16).WithMessage("Your username length must not exceed 10.");

            RuleFor(u => u.Name).NotEmpty().WithMessage("Can't be empty");
            RuleFor(u => u.Surname).NotEmpty().WithMessage("Can't be empty");
            RuleFor(u => u.Phone).NotEmpty().WithMessage("Can't be empty");
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email adress is required").EmailAddress().WithMessage("A valid email is required");
            
            RuleFor(p => p.Password).NotEmpty().WithMessage("Your password cannot be empty")
                   .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                   .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                   .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                   .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                   .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                   .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
        }
    }
}
