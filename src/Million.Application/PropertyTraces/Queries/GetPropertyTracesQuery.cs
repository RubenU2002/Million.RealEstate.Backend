using FluentResults;
using MediatR;
using Million.Application.Properties.DTOs;

namespace Million.Application.PropertyTraces.Queries;

public record GetPropertyTracesQuery(Guid PropertyId) : IRequest<Result<IEnumerable<PropertyTraceDto>>>;
