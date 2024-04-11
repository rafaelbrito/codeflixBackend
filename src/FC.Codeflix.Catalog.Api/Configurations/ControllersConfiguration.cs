using FC.Codeflix.Catalog.Api.Filter;
using FC.Codeflix.Catalog.Infra.Messaging.JsonPolicies;

namespace FC.Codeflix.Catalog.Api.Configurations
{
    public static class ControllersConfiguration
    {
        public static IServiceCollection AddAndConfigureControllers(
            this IServiceCollection services)
        {
            services
                .AddControllers(options
                => options.Filters.Add(typeof(ApiGlobalExceptionFilter))
                )
                .AddJsonOptions(jsonOptions =>
                {
                    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy
                        = new JsonSkaneCasePolicy();
                });
            services.AddDocumentation();
            return services;
        }

        private static IServiceCollection AddDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }
    }
}
