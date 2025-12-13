using FluentResults;
using MediatR;

namespace Million.Application.PropertyTraces.Commands;

public record AddPropertyTraceCommand(
    Guid PropertyId,
    DateTime DateSale,
    string Name,
    decimal Value,
    decimal Tax
) : IRequest<Result<Guid>>;
