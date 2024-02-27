using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UniTests.Comon;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
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

        public DomainEntity.Genre GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null)
        {
            var genre = new DomainEntity.Genre(
                GetValidGenreName(),
                isActive ?? GetRandomBoolean()
                );
            categoriesIds?.ForEach(genre.AddCategory);
            return genre;

        }

        public List<Guid> GetRandomIdsList(int? count = null)
            => Enumerable.Range(1, count ?? (new Random().Next(1, 10)))
                .Select(_ => Guid.NewGuid())
                .ToList();

        public List<DomainEntity.Genre> GetExampleGenreList(int count = 10)
         => Enumerable.Range(1, count).Select(_ =>
            {
                var genre = new DomainEntity.Genre(
                GetValidGenreName(),
                GetRandomBoolean()
                );
                GetRandomIdsList().ForEach(genre.AddCategory);
                return genre;
            }).ToList();
    }
}
