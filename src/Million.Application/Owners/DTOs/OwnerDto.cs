namespace Million.Application.Owners.DTOs;

public record OwnerDto(
    Guid Id,
    string Name,
    string Address,
    string Photo,
    DateTime Birthday
);
