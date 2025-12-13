namespace Million.Application.Properties.DTOs;

public record PropertyTraceDto(
    Guid Id,
    DateTime DateSale,
    string Name,
    decimal Value,
    decimal Tax
);
