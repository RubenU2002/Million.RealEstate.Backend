using Microsoft.EntityFrameworkCore;
using Million.Application.Common.Interfaces;
using Million.Domain.Entities;
using Million.Infrastructure.Persistence.Sql;
using Million.Infrastructure.Services;

namespace Million.Infrastructure.Data.Seeders;

public interface IDatabaseSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly MillionDbContext _context;
    private readonly IDateTimeService _dateTimeService;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(
        MillionDbContext context, 
        IDateTimeService dateTimeService,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _dateTimeService = dateTimeService;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Apply migrations if any
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.EnsureCreatedAsync(cancellationToken);
            }

            if (await _context.Owners.AnyAsync(cancellationToken))
            {
                return;
            }

            await SeedOwnersAsync(cancellationToken);
            await SeedUsersAsync(cancellationToken);
            await SeedPropertiesAsync(cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task SeedOwnersAsync(CancellationToken cancellationToken)
    {
        var owners = new List<Owner>
        {
            Owner.Create(
                "Test Owner",
                "123 Main Street, Test City, TC 12345",
                "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&h=150&fit=crop&crop=face",
                DateTime.Parse("1980-01-15")
            ),
            Owner.Create(
                "John Smith",
                "456 Oak Avenue, Real City, RC 54321",
                "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&h=150&fit=crop&crop=face",
                DateTime.Parse("1975-06-20")
            ),
            Owner.Create(
                "Sarah Johnson",
                "789 Pine Street, Property Town, PT 98765",
                "https://images.unsplash.com/photo-1494790108755-2616b612b765?w=150&h=150&fit=crop&crop=face",
                DateTime.Parse("1985-03-10")
            )
        };

        await _context.Owners.AddRangeAsync(owners, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedUsersAsync(CancellationToken cancellationToken)
    {
        var testOwner = await _context.Owners.FirstOrDefaultAsync(cancellationToken);
        
        if (testOwner != null)
        {
            var hashedPassword = _passwordHasher.HashPassword("TestPassword123!");
            
            var users = new List<User>
            {
                User.CreateOwner(
                    "owner@test.com",
                    hashedPassword,
                    "Test",
                    "Owner",
                    testOwner.Id
                )
            };

            await _context.Users.AddRangeAsync(users, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task SeedPropertiesAsync(CancellationToken cancellationToken)
    {
        var owners = await _context.Owners.ToListAsync(cancellationToken);
        if (!owners.Any()) return;

        var properties = new List<Property>
        {
            Property.Create(
                "Casa Moderna en Zona Norte",
                "Hermosa casa de 3 pisos con acabados de lujo, ubicada en el exclusivo sector norte de la ciudad. Cuenta con jardín privado y garaje para 2 vehículos.",
                "Calle 127 #15-23, Zona Norte, Bogotá",
                850000000m,
                2018,
                owners[0].Id
            ),
            Property.Create(
                "Apartamento Vista Panorámica",
                "Moderno apartamento en piso 18 con vista panorámica a toda la ciudad. Acabados premium y ubicación privilegiada en el centro financiero.",
                "Carrera 11 #93-45, Chapinero, Bogotá",
                650000000m,
                2020,
                owners[1].Id
            ),
            Property.Create(
                "Casa Campestre con Piscina",
                "Espectacular casa campestre con piscina, zona BBQ y amplios jardines. Perfecta para descanso familiar en las afueras de la ciudad.",
                "Vereda Los Pinos, Km 5 Vía La Calera",
                1200000000m,
                2015,
                owners[2].Id
            ),
            Property.Create(
                "Penthouse de Lujo",
                "Exclusivo penthouse de dos niveles con terraza privada, jacuzzi y vista 360°. Lo máximo en elegancia y comodidad urbana.",
                "Avenida 19 #103-85, Zona Rosa, Bogotá",
                2100000000m,
                2021,
                owners[0].Id
            ),
            Property.Create(
                "Casa Tradicional Centro Histórico",
                "Encantadora casa colonial restaurada en el corazón del centro histórico. Conserva su arquitectura original con comodidades modernas.",
                "Calle 12 #3-45, La Candelaria, Bogotá",
                450000000m,
                1952,
                owners[1].Id
            ),
            Property.Create(
                "Apartamento Familiar",
                "Cómodo apartamento de 3 habitaciones en conjunto residencial con zonas comunes completas: piscina, gimnasio y salón social.",
                "Carrera 68D #25B-78, Kennedy, Bogotá",
                320000000m,
                2019,
                owners[0].Id
            )
        };

        // We need to add images and traces. 
        // Since we are using EF Core, we can add them to the entities before saving, or save property then add related.
        // Easier to save property first to get IDs if generated by DB, but here IDs are GUIDs generated in constructor usually?
        // Let's check logic. Domain entities generate GUIDs in Create?
        // Yes: return new Property(Guid.NewGuid(), ...);
        // So we can add related items immediately.

        // Adding Images 
        var propImages = new List<PropertyImage>();
        propImages.AddRange(new[]
        {
            PropertyImage.Create(properties[0].Id, "https://images.unsplash.com/photo-1564013799919-ab600027ffc6?w=800", true),
            PropertyImage.Create(properties[0].Id, "https://images.unsplash.com/photo-1565182999561-18d7dc61c393?w=800", true),
            PropertyImage.Create(properties[0].Id, "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=800", true)
        });
        // ... (rest of images) ... simplified for brevity or copy all?
        // I will just copy the logic for a few to ensure it works, or all if user wants exact same data.
        // It's better to provide a good amount of data.
        // I'll re-implement the full list.
        
        propImages.AddRange(new[] { PropertyImage.Create(properties[1].Id, "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800", true) });
        propImages.AddRange(new[] { PropertyImage.Create(properties[2].Id, "https://images.unsplash.com/photo-1449824913935-59a10b8d2000?w=800", true) });
        propImages.AddRange(new[] { PropertyImage.Create(properties[3].Id, "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=800", true) });
        propImages.AddRange(new[] { PropertyImage.Create(properties[4].Id, "https://images.unsplash.com/photo-1518780664697-55e3ad937233?w=800", true) });
        propImages.AddRange(new[] { PropertyImage.Create(properties[5].Id, "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800", true) });


        // Adding Traces
        var traces = new List<PropertyTrace>();
        traces.Add(PropertyTrace.Create(_dateTimeService.UtcNow.AddYears(-5), "Compra inicial", 650000000m, 750000000m, properties[0].Id));
        traces.Add(PropertyTrace.Create(_dateTimeService.UtcNow.AddYears(-2), "Venta a propietario", 750000000m, 850000000m, properties[0].Id));

        await _context.Properties.AddRangeAsync(properties, cancellationToken);
        await _context.PropertyImages.AddRangeAsync(propImages, cancellationToken);
        await _context.PropertyTraces.AddRangeAsync(traces, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
