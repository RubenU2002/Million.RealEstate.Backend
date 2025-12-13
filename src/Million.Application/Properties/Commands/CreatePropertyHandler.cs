using MediatR;
using Million.Application.Common.Interfaces;
using Million.Application.Common.Extensions;
using Million.Domain.Entities;
using FluentResults;

namespace Million.Application.Properties.Commands;

public class CreatePropertyHandler : IRequestHandler<CreatePropertyCommand, Result<Guid>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreatePropertyHandler(
        IPropertyRepository propertyRepository,
        IOwnerRepository ownerRepository,
        IPropertyImageRepository propertyImageRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _ownerRepository = ownerRepository;
        _propertyImageRepository = propertyImageRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Extraer el Owner ID del token JWT
            var ownerId = _currentUserService.OwnerIdFromToken;
            if (!ownerId.HasValue)
            {
                return ResultExtensions.Unauthorized<Guid>("Owner ID not found in token");
            }

            var ownerExists = await _ownerRepository.ExistsAsync(ownerId.Value, cancellationToken);
            if (!ownerExists)
            {
                return ResultExtensions.BadRequest<Guid>($"Owner with ID {ownerId.Value} does not exist");
            }

            var property = Property.Create(
                request.Name,
                request.Description,
                request.Address,
                request.Price,
                request.Year,
                ownerId.Value
            );

            var propertyId = await _propertyRepository.AddAsync(property, cancellationToken);

            if (request.Images != null && request.Images.Any())
            {
                foreach (var imageFile in request.Images)
                {
                    var propertyImage = PropertyImage.Create(propertyId, imageFile, true);
                    await _propertyImageRepository.AddAsync(propertyImage);
                }
            }

            return Result.Ok(propertyId);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error creating property: {ex.Message}");
        }
    }
}
