using FluentResults;
using Mapster;
using MediatR;
using Million.Application.Common.Extensions;
using Million.Application.Common.Interfaces;
using Million.Application.Properties.DTOs;

namespace Million.Application.Properties.Queries;

public class GetPropertiesByOwnerIdHandler : IRequestHandler<GetPropertiesByOwnerIdQuery, Result<IEnumerable<PropertyDto>>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IPropertyImageRepository _propertyImageRepository;

    public GetPropertiesByOwnerIdHandler(
        IPropertyRepository propertyRepository,
        IOwnerRepository ownerRepository,
        IPropertyImageRepository propertyImageRepository)
    {
        _propertyRepository = propertyRepository;
        _ownerRepository = ownerRepository;
        _propertyImageRepository = propertyImageRepository;
    }

    public async Task<Result<IEnumerable<PropertyDto>>> Handle(GetPropertiesByOwnerIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el owner existe
            var owner = await _ownerRepository.GetByIdAsync(request.OwnerId, cancellationToken);
            if (owner == null)
            {
                return ResultExtensions.NotFound<IEnumerable<PropertyDto>>($"Owner with ID {request.OwnerId} not found");
            }

            var properties = await _propertyRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);
            var result = new List<PropertyDto>();

            foreach (var property in properties)
            {
                var images = await _propertyImageRepository.GetByPropertyIdAsync(property.Id, cancellationToken);

                result.Add(new PropertyDto(
                    property.Id,
                    property.OwnerId,
                    owner.Name,
                    property.Name,
                    property.Description,
                    property.Address,
                    property.Price,
                    property.CodeInternal,
                    property.Year,
                    property.Created,
                    images.Adapt<IEnumerable<PropertyImageDto>>()
                ));
            }
            
            return Result.Ok(result.AsEnumerable());
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving properties for owner: {ex.Message}");
        }
    }
}
