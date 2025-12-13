using Mapster;
using Million.Application.Owners.DTOs;
using Million.Application.Properties.DTOs;
using Million.Domain.Entities;

namespace Million.Application.Common.Mapping;

public static class RegisterMappings
{
    public static void ConfigureMappings()
    {
        TypeAdapterConfig<Owner, OwnerDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Address, src => src.Address)
            .Map(dest => dest.Photo, src => src.Photo)
            .Map(dest => dest.Birthday, src => src.Birthday);

        TypeAdapterConfig<Property, PropertyDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.OwnerId, src => src.OwnerId)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Address, src => src.Address)
            .Map(dest => dest.Price, src => src.Price)
            .Map(dest => dest.CodeInternal, src => src.CodeInternal)
            .Map(dest => dest.Year, src => src.Year)
            .Map(dest => dest.Created, src => src.Created)
            .Map(dest => dest.OwnerName, src => "")
            .Map(dest => dest.Images, src => new List<PropertyImageDto>());

        TypeAdapterConfig<PropertyImage, PropertyImageDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.File, src => src.File)
            .Map(dest => dest.Enabled, src => src.Enabled);

        TypeAdapterConfig<PropertyTrace, PropertyTraceDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.DateSale, src => src.DateSale)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.Tax, src => src.Tax);
    }
}
