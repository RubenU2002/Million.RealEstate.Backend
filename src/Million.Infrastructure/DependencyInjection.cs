using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Million.Application.Common.Interfaces;
using Million.Infrastructure.Common;
using Million.Infrastructure.Data.Seeders;
using Million.Infrastructure.Persistence.Sql;
using Million.Infrastructure.Persistence.Sql.Repositories;
using Million.Infrastructure.Services;

namespace Million.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuración de SQL Server
        services.AddDbContext<MillionDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(MillionDbContext).Assembly.FullName)));

        // Configuración de JWT
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        // Registrar repositorios
        services.AddScoped<IOwnerRepository, OwnerRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
        services.AddScoped<IPropertyTraceRepository, PropertyTraceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Registrar servicios de infraestructura
        services.AddTransient<IDateTimeService, DateTimeService>();
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IFileService, LocalFileService>();

        // Registrar seeder
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

        return services;
    }
}
