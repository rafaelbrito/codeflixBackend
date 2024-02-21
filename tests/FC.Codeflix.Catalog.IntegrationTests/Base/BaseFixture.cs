using Bogus;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.IntegrationTests.Base
{
    public class BaseFixture
    {
        protected Faker Faker {  get; set; }

        public BaseFixture()
            => Faker = new Faker("pt_BR");

        public CodeflixCatalogDbContext CreateDbContext(bool preserveData = false)
        {
            var dbContext = new CodeflixCatalogDbContext(
                new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                .UseInMemoryDatabase(databaseName: "integration-tests-db")
                .Options
                );
            if (preserveData == false)
                dbContext.Database.EnsureDeleted();
            return dbContext;
        }
    }
}
