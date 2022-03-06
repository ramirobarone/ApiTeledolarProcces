using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.CQRS.Auth.Queries.Login
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public LoggedUserData UserData { get; set; }
    }

    public class LoggedUserData : IMapFrom<User>
    {
        public string Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, LoggedUserData>();
        }
    }

}
