using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Security
{
    public class JwtGenerator : IJwtGenerator
    {

        private readonly SymmetricSecurityKey _key;
        private readonly DateTime _expires;
        private readonly string _issuer;
        public JwtGenerator(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value));
            _expires = DateTime.Now.AddDays(Convert.ToDouble(config.GetSection("Jwt:ExpireDays").Value));
            _issuer = config.GetSection("Jwt:Issuer").Value;
        }
        public string CreateToken(User user)
        {
            var claims = new List<Claim> {
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid ().ToString ()),
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString ()),
                new Claim (ClaimTypes.Name, user.Username),
            };

            var token = new JwtSecurityToken(
                _issuer,
                _issuer,
                claims,
                expires: _expires,
                signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
