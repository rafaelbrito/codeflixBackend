using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UnitOfWorkInfra = FC.Codeflix.Catalog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork
{
    [Collection(nameof(UnitOfWorkTestFixture))]
    public class UnitOfWorkTest
    {
        private readonly UnitOfWorkTestFixture _fixute;

        public UnitOfWorkTest(UnitOfWorkTestFixture fixture)
        => _fixute = fixture;

        [Fact(DisplayName = nameof(Commit))]
        [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
        public async Task Commit()
        {
            var dbContext = _fixute.CreateDbContext();
            var exampleCategoryList = _fixute.GetExampleCategoryList();
            await dbContext.AddRangeAsync(exampleCategoryList);
            var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);

            await unitOfWork.Commit(CancellationToken.None);

            var assertDbContext = _fixute.CreateDbContext(true);
            var Savedcategories = assertDbContext.Categories.AsNoTracking().ToList();

            Savedcategories.Should().HaveCount(exampleCategoryList.Count);
        }

        [Fact(DisplayName = nameof(Rollback))]
        [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
        public async Task Rollback()
        {
            var dbContext = _fixute.CreateDbContext();
            var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);

            var task = async () => await unitOfWork.Rollback(CancellationToken.None);

            await task.Should().NotThrowAsync();
        }
    }
}

