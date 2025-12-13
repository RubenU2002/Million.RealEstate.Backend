namespace Million.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserEmail { get; }
    string? UserRole { get; }
    Guid? OwnerIdFromToken { get; }
    bool IsOwner { get; }
    bool IsClient { get; }
}
