using FluentResults;
using MediatR;
using Million.Application.Owners.DTOs;

namespace Million.Application.Owners.Queries;

public record GetAllOwnersQuery : IRequest<Result<IEnumerable<OwnerDto>>>;
