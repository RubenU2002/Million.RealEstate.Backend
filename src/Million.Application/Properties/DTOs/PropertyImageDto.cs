namespace Million.Application.Properties.DTOs;

public record PropertyImageDto(
    Guid Id,
    string File,
    bool Enabled
);
