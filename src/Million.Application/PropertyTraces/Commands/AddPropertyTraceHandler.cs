using FluentResults;
using MediatR;
using Million.Application.Common.Extensions;
using Million.Application.Common.Interfaces;
using Million.Domain.Entities;

namespace Million.Application.PropertyTraces.Commands;

public class AddPropertyTraceHandler : IRequestHandler<AddPropertyTraceCommand, Result<Guid>>
{
    private readonly IPropertyTraceRepository _propertyTraceRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddPropertyTraceHandler(
        IPropertyTraceRepository propertyTraceRepository,
        IPropertyRepository propertyRepository,
        ICurrentUserService currentUserService)
    {
        _propertyTraceRepository = propertyTraceRepository;
        _propertyRepository = propertyRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(AddPropertyTraceCommand request, CancellationToken cancellationToken)
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
                return ResultExtensions.Forbidden<Guid>("You can only add traces to your own properties");
            }

            var propertyTrace = PropertyTrace.Create(
                request.DateSale,
                request.Name,
                request.Value,
                request.Tax,
                request.PropertyId
            );

            await _propertyTraceRepository.AddAsync(propertyTrace);

            return Result.Ok(propertyTrace.Id);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error adding property trace: {ex.Message}");
        }
    }
}
