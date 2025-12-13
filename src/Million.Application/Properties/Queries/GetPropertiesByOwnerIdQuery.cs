using FluentResults;
using MediatR;
using Million.Application.Properties.DTOs;

namespace Million.Application.Properties.Queries;

public record GetPropertiesByOwnerIdQuery(Guid OwnerId) : IRequest<Result<IEnumerable<PropertyDto>>>;
