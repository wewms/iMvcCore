using System;
using System.Security.Cryptography;
using System.Text;
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
                if(!string.IsNullOrEmpty(options.DefaultConnection))
                {
                    var bytes = Convert.FromBase64String(options.DefaultConnection);
                    options.DefaultConnection = Encoding.UTF8.GetString(rsa.Decrypt(bytes, RSAEncryptionPadding.OaepSHA256));
                }

                if(!string.IsNullOrEmpty(options.Connection1))
                {
                    var bytes = Convert.FromBase64String(options.Connection1);
                    options.Connection1 = Encoding.UTF8.GetString(rsa.Decrypt(bytes, RSAEncryptionPadding.OaepSHA256));
                }

                if(!string.IsNullOrEmpty(options.Connection2))
                {
                    var bytes = Convert.FromBase64String(options.Connection2);
                    options.Connection2 = Encoding.UTF8.GetString(rsa.Decrypt(bytes, RSAEncryptionPadding.OaepSHA256));
                }

                if(!string.IsNullOrEmpty(options.Connection3))
                {
                    var bytes = Convert.FromBase64String(options.Connection3);
                    options.Connection3 = Encoding.UTF8.GetString(rsa.Decrypt(bytes, RSAEncryptionPadding.OaepSHA256));
                }

                if(!string.IsNullOrEmpty(options.Connection4))
                {
                    var bytes = Convert.FromBase64String(options.Connection4);
                    options.Connection4 = Encoding.UTF8.GetString(rsa.Decrypt(bytes, RSAEncryptionPadding.OaepSHA256));
                }

                if(!string.IsNullOrEmpty(options.Connection5))
                {
                    var bytes = Convert.FromBase64String(options.Connection5);
                    options.Connection5 = Encoding.UTF8.GetString(rsa.Decrypt(bytes, RSAEncryptionPadding.OaepSHA256));
                }
            });

            return services;
        }
    }
}