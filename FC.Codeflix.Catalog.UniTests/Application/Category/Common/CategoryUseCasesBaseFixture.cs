using FC.Codeflix.Catalog.Application.Inferfaces;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UniTests.Comon;
using Moq;

namespace FC.Codeflix.Catalog.UniTests.Application.Category.Common
{
    public abstract class CategoryUseCasesBaseFixture : BaseFixture
    {
        public string GetValidCategoryName()
        {
            var categoryName = "";
            while (categoryName.Length < 3)
                categoryName = Faker.Commerce.Categories(1)[0];
            if (categoryName.Length > 255)
                categoryName = categoryName[..255];
            return categoryName;
        }

        public string GetValidCategoryDescription()
        {
            var categoryDescription = Faker.Commerce.ProductDescription();
            if (categoryDescription.Length > 255)
                categoryDescription = categoryDescription[..10_000];
            return categoryDescription;
        }

        public Mock<ICategoryRepository> GetRepositoryMock() => new();

        public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

        public DomainEntity.Category GetExampleCategory()
     => new(GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );
    }
}
