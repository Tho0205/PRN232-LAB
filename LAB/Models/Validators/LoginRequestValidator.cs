using FluentValidation;
using PRN232.LAB.API.Models.Requests;

namespace PRN232.LAB.API.Models.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(50)
                .Must(username => !username.Contains(' '))
                .WithMessage("Username cannot contain spaces.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(100);
        }
    }
}
