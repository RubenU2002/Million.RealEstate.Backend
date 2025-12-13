using FluentResults;
using Mapster;
using MediatR;
using Million.Application.Common.Interfaces;
using Million.Application.Owners.DTOs;

namespace Million.Application.Owners.Queries;

public class GetAllOwnersHandler : IRequestHandler<GetAllOwnersQuery, Result<IEnumerable<OwnerDto>>>
{
    private readonly IOwnerRepository _ownerRepository;

    public GetAllOwnersHandler(IOwnerRepository ownerRepository)
    {
        _ownerRepository = ownerRepository;
    }

    public async Task<Result<IEnumerable<OwnerDto>>> Handle(GetAllOwnersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var owners = await _ownerRepository.GetAllAsync(cancellationToken);
            var ownerDtos = owners.Adapt<IEnumerable<OwnerDto>>();
            return Result.Ok(ownerDtos);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving owners: {ex.Message}");
        }
    }
}
