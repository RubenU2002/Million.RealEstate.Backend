namespace Million.Application.Auth.DTOs;

public record LoginRequest(string Email, string Password);

public record AuthResponse(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    Guid? OwnerId,
    string Token
);
