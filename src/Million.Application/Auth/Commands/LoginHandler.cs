using MediatR;
using Million.Application.Auth.DTOs;
using Million.Application.Common.Interfaces;
using Million.Application.Common.Extensions;
using FluentResults;

namespace Million.Application.Auth.Commands;

public class LoginHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (user == null || !user.IsActive)
        {
            return ResultExtensions.Unauthorized<AuthResponse>("Invalid email or password");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return ResultExtensions.Unauthorized<AuthResponse>("Invalid email or password");
        }

        var token = _tokenGenerator.GenerateToken(user);

        var response = new AuthResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role,
            user.OwnerId,
            token
        );

        return Result.Ok(response);
    }
}
