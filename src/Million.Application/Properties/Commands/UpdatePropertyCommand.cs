using FluentResults;
using MediatR;

namespace Million.Application.Properties.Commands;

public record UpdatePropertyCommand(
    Guid PropertyId,
    string Name,
    string Description,
    string Address,
    decimal Price,
    int Year
) : IRequest<Result>;
