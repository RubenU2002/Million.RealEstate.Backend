using FluentResults;
using MediatR;
using Million.Application.Common.Extensions;
using Million.Application.Common.Interfaces;

namespace Million.Application.Properties.Commands;

public class UpdatePropertyHandler : IRequestHandler<UpdatePropertyCommand, Result>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdatePropertyHandler(
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentOwnerId = _currentUserService.OwnerIdFromToken;
            if (currentOwnerId == null)
            {
                return ResultExtensions.Unauthorized("Owner ID not found in token");
            }

            var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
            
            if (property == null)
            {
                return ResultExtensions.NotFound($"Property with ID {request.PropertyId} not found");
            }

            if (property.OwnerId != currentOwnerId)
            {
                return ResultExtensions.Forbidden("You can only update your own properties");
            }

            // Actualizar la propiedad
            property.UpdateDetails(
                request.Name,
                request.Description,
                request.Address,
                request.Price,
                request.Year
            );

            var updated = await _propertyRepository.UpdateAsync(property, cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating property: {ex.Message}");
        }
    }
}
