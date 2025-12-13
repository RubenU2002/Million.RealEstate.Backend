using MediatR;
using Million.Application.Auth.DTOs;
using FluentResults;

namespace Million.Application.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
