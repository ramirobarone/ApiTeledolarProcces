using FluentValidation;

namespace Application.CQRS.Auth.Queries.Login
{
    public class LoginQueryValidator : AbstractValidator<LoginRequest>
    {
        public LoginQueryValidator()
        {
            RuleFor(x => x.Usuario).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
