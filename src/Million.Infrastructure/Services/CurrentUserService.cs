using Microsoft.AspNetCore.Http;
using Million.Application.Common.Interfaces;
using Million.Domain.Entities;
using System.Security.Claims;

namespace Million.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }
    }

    public string? UserEmail => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? UserRole => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

    public Guid? OwnerIdFromToken
    {
        get
        {
            var ownerIdString = _httpContextAccessor.HttpContext?.User?.FindFirst("ownerId")?.Value;
            return Guid.TryParse(ownerIdString, out var ownerId) ? ownerId : null;
        }
    }

    public bool IsOwner => UserRole == UserRoles.Owner;

    public bool IsClient => UserRole == UserRoles.Client;
}
