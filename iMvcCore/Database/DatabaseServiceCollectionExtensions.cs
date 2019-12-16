using iMvcCore.Extensions;
using iMvcCore.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iMvcCore.Database
{
    public static class DatabaseServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // Registers databases
            services.AddScoped<Database>();
            services.AddScoped<Database1>();
            services.AddScoped<Database2>();
            services.AddScoped<Database3>();
            services.AddScoped<Database4>();
            services.AddScoped<Database5>();

            // Configures database options
            services.Configure<DatabaseOptions>(configuration);

            // Decrypts database connection string
            var rsa = X509.GetRSAPrivateKey(configuration.GetValue<string>(X509.CertFileName), configuration.GetValue<string>(X509.CertFileKey));
            services.Configure<DatabaseOptions>(options => {
                if(!string.IsNullOrEmpty(options.DefaultConnection)) options.DefaultConnection = rsa.Decrypt(options.DefaultConnection);
                if(!string.IsNullOrEmpty(options.Connection1)) options.Connection1 = rsa.Decrypt(options.Connection1);
                if(!string.IsNullOrEmpty(options.Connection2)) options.Connection2 = rsa.Decrypt(options.Connection2);
                if(!string.IsNullOrEmpty(options.Connection3)) options.Connection3 = rsa.Decrypt(options.Connection3);
                if(!string.IsNullOrEmpty(options.Connection4)) options.Connection4 = rsa.Decrypt(options.Connection4);
                if(!string.IsNullOrEmpty(options.Connection5)) options.Connection5 = rsa.Decrypt(options.Connection5);
            });

            return services;
        }
    }
}