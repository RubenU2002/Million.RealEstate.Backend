using MediatR;
using Million.Application.Common.Models;
using Million.Application.Properties.DTOs;
using FluentResults;

namespace Million.Application.Properties.Queries;

public record SearchPropertiesQuery(
    string? Name = null,
    string? Address = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedResult<PropertyDto>>>;
