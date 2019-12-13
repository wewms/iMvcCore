using Microsoft.Extensions.DependencyInjection;

namespace iMvcCore.Logging
{
    public static class AppLoggerServiceCollectionExtensions
    {
        public static void AddAppLogger(this IServiceCollection services)
        {
            services.AddSingleton<AppLogger>();
        }
    }
}