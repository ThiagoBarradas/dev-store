using DevStore.Identity.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Identity.Jwt;
using NetDevPack.Security.PasswordHasher.Core;

namespace DevStore.Identity.API.Configuration;

public static class IdentityConfig
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services
                .AddMemoryCache()
                .AddDataProtection();
        services.AddJwtConfiguration(configuration)
                .AddNetDevPackIdentity<IdentityUser>()
                .PersistKeysToDatabaseStore<ApplicationDbContext>();

        services.AddIdentity<IdentityUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequiredUniqueChars = 0;
                o.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.UpgradePasswordSecurity()
            .WithStrenghten(PasswordHasherStrenght.Moderate)
            .UseArgon2<IdentityUser>();

        return services;
    }
}