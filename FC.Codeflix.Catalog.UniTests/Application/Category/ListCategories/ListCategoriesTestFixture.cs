using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.UniTests.Application.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Category.ListCategories
{
    [CollectionDefinition(nameof(ListCategoriesTestFixture))]
    public class ListCategoriesFixtrureCollection : ICollectionFixture<ListCategoriesTestFixture>
    { }

    public class ListCategoriesTestFixture : CategoryUseCasesBaseFixture
    {
        public List<DomainEntity.Category> GetExampleCategoryList(int lengt = 10)
        {
            var list = new List<DomainEntity.Category>();
            for (int i = 0; i < lengt; i++)
                list.Add(GetExampleCategory());
            return list;
        }

        public ListCategoriesInput GetExampleInput()
        {
            var random = new Random();
            return new ListCategoriesInput(
                page: random.Next(1, 10),
                perPage: random.Next(15, 100),
                search: Faker.Commerce.ProductName(),
                sort: Faker.Commerce.ProductName(),
                dir: random.Next(0, 10) > 5 ?
                SearchOrder.Asc : SearchOrder.Desc);
        }
    }
}
