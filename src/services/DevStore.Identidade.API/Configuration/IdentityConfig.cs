using DevStore.Identidade.API.Data;
using DevStore.Identidade.API.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetDevPack.Security.JwtSigningCredentials;
using NetDevPack.Security.PasswordHasher.Core;

namespace DevStore.Identidade.API.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppTokenDevStoreSettings");
            services.Configure<AppTokeDevStorettings>(appSettingsSection);

            services.AddJwksManager(options => options.Jws = JwsAlgorithm.ES256);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddErrorDescriber<IdentityMensagensPortugues>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.UpgradePasswordSecurity()
                .WithStrenghten(PasswordHasherStrenght.Moderate)
                .UseArgon2<IdentityUser>();

            return services;
        }
    }
}