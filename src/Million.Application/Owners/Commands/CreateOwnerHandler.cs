using FluentResults;
using Mapster;
using MediatR;
using Million.Application.Common.Extensions;
using Million.Application.Common.Interfaces;
using Million.Application.Owners.DTOs;
using Million.Domain.Entities;

namespace Million.Application.Owners.Commands;

public class CreateOwnerHandler : IRequestHandler<CreateOwnerCommand, Result<OwnerDto>>
{
    private readonly IOwnerRepository _ownerRepository;

    public CreateOwnerHandler(IOwnerRepository ownerRepository)
    {
        _ownerRepository = ownerRepository;
    }

    public async Task<Result<OwnerDto>> Handle(CreateOwnerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var owner = new Owner(
                Guid.NewGuid(),
                request.Name,
                request.Address,
                request.Photo,
                request.Birthday
            );

            await _ownerRepository.AddAsync(owner, cancellationToken);

            var ownerDto = owner.Adapt<OwnerDto>();
            return Result.Ok(ownerDto);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error creating owner: {ex.Message}");
        }
    }
}
