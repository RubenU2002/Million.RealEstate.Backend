using FluentResults;
using MediatR;
using Million.Application.Common.Extensions;
using Million.Application.Common.Interfaces;
using Million.Domain.Entities;

namespace Million.Application.PropertyImages.Commands;

public class AddPropertyImageHandler : IRequestHandler<AddPropertyImageCommand, Result<Guid>>
{
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddPropertyImageHandler(
        IPropertyImageRepository propertyImageRepository,
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService)
    {
        _propertyImageRepository = propertyImageRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(AddPropertyImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Extraer el Owner ID del token JWT
            var ownerId = _currentUserService.OwnerIdFromToken;
            if (!ownerId.HasValue)
            {
                return ResultExtensions.Unauthorized<Guid>("Owner ID not found in token");
            }

            // Verificar que la propiedad existe
            var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
            if (property == null)
            {
                return ResultExtensions.NotFound<Guid>($"Property with ID {request.PropertyId} not found");
            }

            // Verificar que el usuario es el owner de la propiedad
            if (property.OwnerId != ownerId.Value)
            {
                return ResultExtensions.Forbidden<Guid>("You can only add images to your own properties");
            }

            var propertyImage = PropertyImage.Create(
                request.PropertyId,
                request.File,
                request.Enabled
            );

            await _propertyImageRepository.AddAsync(propertyImage);

            return Result.Ok(propertyImage.Id);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error adding property image: {ex.Message}");
        }
    }
}
