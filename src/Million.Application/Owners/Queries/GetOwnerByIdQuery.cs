using FluentResults;
using MediatR;
using Million.Application.Owners.DTOs;

namespace Million.Application.Owners.Queries;

public record GetOwnerByIdQuery(Guid Id) : IRequest<Result<OwnerDto>>;
