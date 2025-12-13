namespace Million.Application.Properties.DTOs;

public record PropertySearchCriteriaDto(
    string? Name = null,
    string? Address = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null
);
