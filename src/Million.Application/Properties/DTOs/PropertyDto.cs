namespace Million.Application.Properties.DTOs;

public record PropertyDto(
    Guid Id,
    Guid OwnerId,
    string OwnerName,
    string Name,
    string Description,
    string Address,
    decimal Price,
    string CodeInternal,
    int Year,
    DateTime Created,
    IEnumerable<PropertyImageDto> Images
);
