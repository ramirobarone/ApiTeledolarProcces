using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            Id = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Id != null)
            {
                AuthorizedUserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                AuthorizedUserName = httpContextAccessor.HttpContext?.User?.Identity?.Name;
            }
            IsAuthenticated = AuthorizedUserId != null;
        }
        private string Id { get; set; }
        public string AuthorizedUserName { get; }
        public string AuthorizedUserId { get; }
        public bool IsAuthenticated { get; }
    }
}
