using FluentResults;
using MediatR;
using Million.Application.Owners.DTOs;

namespace Million.Application.Owners.Commands;

public record CreateOwnerCommand(
    string Name,
    string Address,
    string? Photo,
    DateTime Birthday
) : IRequest<Result<OwnerDto>>;
