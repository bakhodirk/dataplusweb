using Microsoft.Extensions.DependencyInjection;


namespace DataPlusWeb.Shared.Services
{
    public static class HttpServiceCollectionExtensions
    {
        public static IServiceCollection DataPlusServiceWeb(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<IDataPlusService, DataPlusService>();

            services.AddHttpClient("DataPlusAPI", options =>
            {
                options.BaseAddress = new Uri("https://localhost:7021/");
            });

            return services;
        }


    }
}
