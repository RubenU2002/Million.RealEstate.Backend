using MediatR;
using Million.Application.Common.Interfaces;
using Million.Application.Common.Models;
using Million.Application.Properties.DTOs;
using FluentResults;
using Mapster;

namespace Million.Application.Properties.Queries;

public class SearchPropertiesHandler : IRequestHandler<SearchPropertiesQuery, Result<PaginatedResult<PropertyDto>>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IPropertyImageRepository _propertyImageRepository;

    public SearchPropertiesHandler(
        IPropertyRepository propertyRepository,
        IOwnerRepository ownerRepository,
        IPropertyImageRepository propertyImageRepository)
    {
        _propertyRepository = propertyRepository;
        _ownerRepository = ownerRepository;
        _propertyImageRepository = propertyImageRepository;
    }

    public async Task<Result<PaginatedResult<PropertyDto>>> Handle(SearchPropertiesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var pagination = new PaginationParameters
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var (properties, totalCount) = await _propertyRepository.SearchPagedAsync(
                pagination,
                request.Name,
                request.Address,
                request.MinPrice,
                request.MaxPrice,
                cancellationToken);

            var result = new List<PropertyDto>();

            foreach (var property in properties)
            {
                var owner = await _ownerRepository.GetByIdAsync(property.OwnerId, cancellationToken);
                var images = await _propertyImageRepository.GetByPropertyIdAsync(property.Id, cancellationToken);

                result.Add(new PropertyDto(
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
                    images.Adapt<IEnumerable<PropertyImageDto>>()
                ));
            }

            var paginatedResult = new PaginatedResult<PropertyDto>(
                result,
                totalCount,
                pagination.PageNumber,
                pagination.PageSize);

            return Result.Ok(paginatedResult);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error searching properties: {ex.Message}");
        }
    }
}
