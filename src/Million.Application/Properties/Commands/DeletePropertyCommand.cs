using FluentResults;
using MediatR;

namespace Million.Application.Properties.Commands;

public record DeletePropertyCommand(Guid PropertyId) : IRequest<Result>;
