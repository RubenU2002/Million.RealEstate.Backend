namespace Million.Application.Properties.DTOs;

public record PropertyDetailDto(
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
    IEnumerable<PropertyImageDto> Images,
    IEnumerable<PropertyTraceDto> Traces
);
