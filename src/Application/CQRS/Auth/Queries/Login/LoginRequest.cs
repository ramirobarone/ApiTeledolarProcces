using MediatR;

namespace Application.CQRS.Auth.Queries.Login
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
    }

}
