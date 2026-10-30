using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SmartPark.Models;

namespace SmartPark.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services, IHostEnvironment environment)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roles = new[] { "Administrator", "Manager", "Staff" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                EnsureIdentitySuccess(
                    await roleManager.CreateAsync(new IdentityRole(role)),
                    $"Creating the {role} role");
        }

        var adminEmail = "admin@smartpark.local";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (!environment.IsDevelopment())
        {
            if (admin is not null)
            {
                EnsureIdentitySuccess(await userManager.SetLockoutEnabledAsync(admin, true), "Disabling the demo administrator account");
                EnsureIdentitySuccess(
                    await userManager.SetLockoutEndDateAsync(admin, DateTimeOffset.MaxValue),
                    "Disabling the demo administrator account");
                EnsureIdentitySuccess(
                    await userManager.UpdateSecurityStampAsync(admin),
                    "Invalidating sessions for the demo administrator account");

                if (await userManager.IsInRoleAsync(admin, "Administrator"))
                {
                    EnsureIdentitySuccess(
                        await userManager.RemoveFromRoleAsync(admin, "Administrator"),
                        "Removing the demo administrator role");
                }
            }

            return;
        }

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Smart",
                LastName = "Admin",
                EmailConfirmed = true,
                RegistrskaStevilka = "GO-1234"
            };
            EnsureIdentitySuccess(await userManager.CreateAsync(admin, "Admin!234"), "Creating the local demo administrator");
        }

        if (!await userManager.IsInRoleAsync(admin, "Administrator"))
            EnsureIdentitySuccess(await userManager.AddToRoleAsync(admin, "Administrator"), "Assigning the local demo administrator role");
    }

    private static void EnsureIdentitySuccess(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException($"{operation} failed: {string.Join("; ", result.Errors.Select(error => error.Description))}");
    }

    public static async Task EnsureDemoParkingAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<SmartParkContext>();
        await SeedDemoParkingAsync(ctx);
    }

    public static async Task SyncParkingAsync(
        IServiceProvider services,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<SmartParkContext>();

        var existingParking = await ctx.Parkirisca
            .Include(p => p.ParkirnaMesta)
            .ToListAsync();

        try
        {
            var provider = new LptParkingProvider();
            var parking = await provider.GetParkiriscaAsync(cancellationToken);
            await ApplyParkingAsync(ctx, existingParking, parking, cancellationToken);
            logger.LogInformation("Imported {Count} parking locations from the official LPT pages.", parking.Count);
        }
        catch (Exception lptException)
        {
            logger.LogWarning(lptException, "LPT parking import failed; trying OSM fallback.");

            try
            {
                using var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(15)
                };

                var api = new OverpassApiHelper(httpClient);
                var parking = await api.GetParkiriscaLjubljanaAsync();

                if (parking.Count > 0)
                {
                    await ApplyParkingAsync(ctx, existingParking, parking, cancellationToken);
                    logger.LogInformation("Imported {Count} parking locations from the OSM fallback.", parking.Count);
                }
            }
            catch (Exception fallbackException)
            {
                logger.LogWarning(fallbackException, "OSM parking import failed; keeping the last local snapshot.");
            }
        }
    }

    private static async Task ApplyParkingAsync(
        SmartParkContext ctx,
        List<Parkirisce> existingParking,
        List<Parkirisce> importedParking,
        CancellationToken cancellationToken)
    {
        if (existingParking.All(p => p.Naslov == "Kongresni trg, Ljubljana"))
            ctx.Parkirisca.RemoveRange(existingParking);

        var currentByAddress = existingParking
            .Where(p => p.Naslov != "Kongresni trg, Ljubljana")
            .ToDictionary(p => p.Naslov, StringComparer.OrdinalIgnoreCase);

        foreach (var imported in importedParking)
        {
            if (currentByAddress.TryGetValue(imported.Naslov, out var current))
            {
                current.Latitude = imported.Latitude;
                current.Longitude = imported.Longitude;
                current.SteviloMest = imported.SteviloMest;
                current.CenaNaUro = imported.CenaNaUro;
                current.DelovniCas = imported.DelovniCas;
            }
            else
            {
                ctx.Parkirisca.Add(imported);
            }
        }

        await ctx.SaveChangesAsync(cancellationToken);

        var parks = await ctx.Parkirisca
            .Include(p => p.ParkirnaMesta)
            .ToListAsync(cancellationToken);

        foreach (var parking in parks)
        {
            var existingSpaces = parking.ParkirnaMesta.Count;
            var targetSpaces = Math.Min(Math.Max(parking.SteviloMest, 1), 200);

            for (var number = existingSpaces + 1; number <= targetSpaces; number++)
            {
                ctx.ParkirnaMesta.Add(new ParkirnoMesto
                {
                    ParkirisceId = parking.Id,
                    Stevilka = number,
                    Zasedeno = false,
                    Tip = TipMesta.Navadno
                });
            }
        }

        await ctx.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedDemoParkingAsync(SmartParkContext ctx)
    {
        if (await ctx.Parkirisca.AnyAsync())
            return;

        var parking = new Parkirisce
        {
            Naslov = "Kongresni trg, Ljubljana",
            SteviloMest = 20,
            CenaNaUro = 1.50m,
            DelovniCas = "00:00–24:00",
            Latitude = 46.0502,
            Longitude = 14.5058
        };

        ctx.Parkirisca.Add(parking);
        await ctx.SaveChangesAsync();

        for (var i = 1; i <= parking.SteviloMest; i++)
        {
            ctx.ParkirnaMesta.Add(new ParkirnoMesto
            {
                ParkirisceId = parking.Id,
                Stevilka = i,
                Zasedeno = false,
                Tip = TipMesta.Navadno
            });
        }

        await ctx.SaveChangesAsync();
        Console.WriteLine("Seeded local demo parking because Overpass was unavailable.");
    }
}