using FluentResults;
using MediatR;
using Million.Application.Common.Extensions;
using Million.Application.Common.Interfaces;

namespace Million.Application.Properties.Commands;

public class DeletePropertyHandler : IRequestHandler<DeletePropertyCommand, Result>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeletePropertyHandler(
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService)
    {
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
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
                return ResultExtensions.Forbidden("You can only delete your own properties");
            }

            var deleted = await _propertyRepository.DeleteAsync(request.PropertyId, cancellationToken);
            
            if (!deleted)
            {
                return Result.Fail("Failed to delete property");
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error deleting property: {ex.Message}");
        }
    }
}
