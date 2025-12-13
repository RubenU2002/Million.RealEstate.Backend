using MediatR;
using Million.Application.Properties.DTOs;
using FluentResults;

namespace Million.Application.Properties.Queries;

public record GetPropertyByIdQuery(Guid Id) : IRequest<Result<PropertyDetailDto>>;
