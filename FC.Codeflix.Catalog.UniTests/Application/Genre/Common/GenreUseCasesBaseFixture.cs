using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UniTests.Comon;
using Moq;

namespace FC.Codeflix.Catalog.UniTests.Application.Genre.Common
{
    public abstract class GenreUseCasesBaseFixture : BaseFixture
    {
        public string GetValidGenreName()
            => Faker.Commerce.Categories(1)[0];


        public Mock<IGenreRepository> GetGenreRepositoryMock()
            => new Mock<IGenreRepository>();

        public Mock<IUnitOfWork> GetUnitOfWorkMock()
            => new Mock<IUnitOfWork>();

        public Mock<ICategoryRepository> GetCategoryRepositoryMock()
            => new Mock<ICategoryRepository>();
    }
}
