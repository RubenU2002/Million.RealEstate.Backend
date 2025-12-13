using Mapster;
using MediatR;
using Million.Application.Common.Interfaces;
using Million.Application.Properties.DTOs;
using Million.Application.Common.Extensions;
using FluentResults;

namespace Million.Application.Properties.Queries;

public class GetPropertyByIdHandler : IRequestHandler<GetPropertyByIdQuery, Result<PropertyDetailDto>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly IPropertyTraceRepository _propertyTraceRepository;

    public GetPropertyByIdHandler(
        IPropertyRepository propertyRepository,
        IOwnerRepository ownerRepository,
        IPropertyImageRepository propertyImageRepository,
        IPropertyTraceRepository propertyTraceRepository)
    {
        _propertyRepository = propertyRepository;
        _ownerRepository = ownerRepository;
        _propertyImageRepository = propertyImageRepository;
        _propertyTraceRepository = propertyTraceRepository;
    }

    public async Task<Result<PropertyDetailDto>> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var property = await _propertyRepository.GetByIdAsync(request.Id, cancellationToken);

            if (property == null)
            {
                return ResultExtensions.NotFound<PropertyDetailDto>($"Property with ID {request.Id} not found");
            }

            var owner = await _ownerRepository.GetByIdAsync(property.OwnerId, cancellationToken);
            var images = await _propertyImageRepository.GetByPropertyIdAsync(property.Id, cancellationToken);
            var traces = await _propertyTraceRepository.GetByPropertyIdAsync(property.Id, cancellationToken);

            var result = new PropertyDetailDto(
                property.Id,
                property.OwnerId,
                owner?.Name ?? "Unknown",
                property.Name,
                property.Description,
                property.Address,
                property.Price,
                property.CodeInternal,
                property.Year,
                property.Created,
                images.Adapt<IEnumerable<PropertyImageDto>>(),
                traces.Adapt<IEnumerable<PropertyTraceDto>>()
            );

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving property: {ex.Message}");
        }
    }
}
