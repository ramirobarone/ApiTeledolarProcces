using Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Auth.Queries.Login
{
    public class LoginQuery : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IMapper _mapper;
        private readonly IJwtGenerator _jwt;
        private readonly IEfectivoYaDoDbContext _context;
        public LoginQuery(IEfectivoYaDoDbContext context, IMapper mapper, IJwtGenerator jwt)
        {
            _context = context;
            _mapper = mapper;
            _jwt = jwt;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Where(x => x.Username == request.Usuario && x.Password == request.Password).FirstOrDefaultAsync();
            if (user == null) throw new UnauthorizedAccessException("Credenciales invalidas");
            var userLogged = new LoginResponse
            {
                Token = _jwt.CreateToken(user),
                UserData = _mapper.Map<LoggedUserData>(user)
            };
            return userLogged;
        }
    }
}
