using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Million.Application.Common.Behaviors;
using Million.Application.Common.Mapping;
using System.Reflection;

namespace Million.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Registrar MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Registrar FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Registrar Mapster
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        RegisterMappings.ConfigureMappings();
        services.AddSingleton(mappingConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
