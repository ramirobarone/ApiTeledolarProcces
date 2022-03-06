namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string AuthorizedUserName { get; }
        string AuthorizedUserId { get; }
        bool IsAuthenticated { get; }
    }
}
