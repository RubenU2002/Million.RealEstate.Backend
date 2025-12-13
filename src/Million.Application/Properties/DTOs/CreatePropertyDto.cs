namespace Million.Application.Properties.DTOs;

public record CreatePropertyDto(
    string Name,
    string Description,
    string Address,
    decimal Price,
    decimal Size,
    int Year
);
