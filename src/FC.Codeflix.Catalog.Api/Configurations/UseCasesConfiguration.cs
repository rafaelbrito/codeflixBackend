using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.Api.Configurations
{
    public static class UseCasesConfiguration
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {

            services.AddMediatR(config => config
            .RegisterServicesFromAssemblies(typeof(CreateCategory).Assembly));
            services.AddRepositories();
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<ICategoryRepository, CategoryRespository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IGenreRepository, GenreRepository>();

            return services;
        }
    }
}
