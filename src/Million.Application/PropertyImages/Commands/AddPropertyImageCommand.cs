using FluentResults;
using MediatR;

namespace Million.Application.PropertyImages.Commands;

public record AddPropertyImageCommand(
    Guid PropertyId,
    string File,
    bool Enabled = true
) : IRequest<Result<Guid>>;
