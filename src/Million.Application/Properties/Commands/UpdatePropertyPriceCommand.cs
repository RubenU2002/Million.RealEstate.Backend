using MediatR;
using FluentResults;

namespace Million.Application.Properties.Commands;

public record UpdatePropertyPriceCommand(
    Guid PropertyId,
    decimal NewPrice
) : IRequest<Result>;
