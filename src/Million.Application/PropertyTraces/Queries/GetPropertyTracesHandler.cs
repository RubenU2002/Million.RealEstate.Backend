using FluentResults;
using MediatR;
using Million.Application.Common.Extensions;
using Million.Application.Common.Interfaces;
using Million.Application.Properties.DTOs;
using Mapster;

namespace Million.Application.PropertyTraces.Queries;

public class GetPropertyTracesHandler : IRequestHandler<GetPropertyTracesQuery, Result<IEnumerable<PropertyTraceDto>>>
{
    private readonly IPropertyTraceRepository _propertyTraceRepository;
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyTracesHandler(
        IPropertyTraceRepository propertyTraceRepository,
        IPropertyRepository propertyRepository)
    {
        _propertyTraceRepository = propertyTraceRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<IEnumerable<PropertyTraceDto>>> Handle(GetPropertyTracesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
            if (property == null)
            {
                return ResultExtensions.NotFound<IEnumerable<PropertyTraceDto>>($"Property with ID {request.PropertyId} not found");
            }

            var traces = await _propertyTraceRepository.GetByPropertyIdAsync(request.PropertyId);

            var traceDtos = traces.Adapt<IEnumerable<PropertyTraceDto>>();

            return Result.Ok(traceDtos);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving property traces: {ex.Message}");
        }
    }
}
