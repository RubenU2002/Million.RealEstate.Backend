using MediatR;
using FluentResults;

namespace Million.Application.Properties.Commands;

public record CreatePropertyCommand(
    string Name,
    string Description,
    string Address,
    decimal Price,
    int Year,
    List<string>? Images = null
) : IRequest<Result<Guid>>;
