using FluentResults;
using MediatR;
using Million.Application.Common.Extensions;
using Million.Application.Common.Interfaces;
using Million.Application.Properties.DTOs;
using Mapster;

namespace Million.Application.PropertyImages.Queries;

public class GetPropertyImagesHandler : IRequestHandler<GetPropertyImagesQuery, Result<IEnumerable<PropertyImageDto>>>
{
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyImagesHandler(
        IPropertyImageRepository propertyImageRepository,
        IPropertyRepository propertyRepository)
    {
        _propertyImageRepository = propertyImageRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<IEnumerable<PropertyImageDto>>> Handle(GetPropertyImagesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
            if (property == null)
            {
                return ResultExtensions.NotFound<IEnumerable<PropertyImageDto>>($"Property with ID {request.PropertyId} not found");
            }

            var images = await _propertyImageRepository.GetByPropertyIdAsync(request.PropertyId);

            var imageDtos = images.Adapt<IEnumerable<PropertyImageDto>>();

            return Result.Ok(imageDtos);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving property images: {ex.Message}");
        }
    }
}
