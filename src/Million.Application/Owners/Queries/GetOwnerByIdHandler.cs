using FluentResults;
using Mapster;
using MediatR;
using Million.Application.Common.Extensions;
using Million.Application.Common.Interfaces;
using Million.Application.Owners.DTOs;

namespace Million.Application.Owners.Queries;

public class GetOwnerByIdHandler : IRequestHandler<GetOwnerByIdQuery, Result<OwnerDto>>
{
    private readonly IOwnerRepository _ownerRepository;

    public GetOwnerByIdHandler(IOwnerRepository ownerRepository)
    {
        _ownerRepository = ownerRepository;
    }

    public async Task<Result<OwnerDto>> Handle(GetOwnerByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var owner = await _ownerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (owner == null)
            {
                return ResultExtensions.NotFound<OwnerDto>($"Owner with ID {request.Id} not found");
            }

            var ownerDto = owner.Adapt<OwnerDto>();
            return Result.Ok(ownerDto);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving owner: {ex.Message}");
        }
    }
}
