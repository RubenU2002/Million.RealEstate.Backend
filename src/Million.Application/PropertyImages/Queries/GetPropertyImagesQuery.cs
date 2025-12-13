using FluentResults;
using MediatR;
using Million.Application.Properties.DTOs;

namespace Million.Application.PropertyImages.Queries;

public record GetPropertyImagesQuery(Guid PropertyId) : IRequest<Result<IEnumerable<PropertyImageDto>>>;
