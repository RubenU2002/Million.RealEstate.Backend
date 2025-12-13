namespace Million.Application.Owners.DTOs;

public record CreateOwnerDto(
    string Name,
    string Address,
    string? Photo,
    DateTime Birthday
);
