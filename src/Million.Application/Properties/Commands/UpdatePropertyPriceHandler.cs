using MediatR;
using Million.Application.Common.Extensions;
using Million.Application.Common.Interfaces;
using FluentResults;

namespace Million.Application.Properties.Commands;

public class UpdatePropertyPriceHandler : IRequestHandler<UpdatePropertyPriceCommand, Result>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdatePropertyPriceHandler(
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdatePropertyPriceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Extraer el Owner ID del token JWT
            var ownerId = _currentUserService.OwnerIdFromToken;
            if (!ownerId.HasValue)
            {
                return ResultExtensions.Unauthorized("Owner ID not found in token");
            }

            var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);

            if (property == null)
            {
                return ResultExtensions.NotFound($"Property with ID {request.PropertyId} not found");
            }

            // Verificar que el usuario es el owner de la propiedad
            if (property.OwnerId != ownerId.Value)
            {
                return ResultExtensions.Forbidden("You can only update the price of your own properties");
            }

            property.ChangePrice(request.NewPrice);
            
            await _propertyRepository.UpdateAsync(property, cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error updating property price: {ex.Message}");
        }
    }
}
