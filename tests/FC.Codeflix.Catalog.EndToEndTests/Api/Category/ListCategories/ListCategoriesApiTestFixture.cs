using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories
{
    [CollectionDefinition(nameof(ListCategoriesApiTestFixture))]
    public class ListCategoriesApiTestFixtureCollection : ICollectionFixture<ListCategoriesApiTestFixture>
    { }

    public class ListCategoriesApiTestFixture : CategoryBaseFixture
    {

        public List<DomainEntity.Category> GetExampleCategoriesListWithNames(List<string> names)
            => names.Select(name =>
            {
                var category = GetExampleCategory();
                category.Update(name);
                return category;
            }).ToList();

        public List<DomainEntity.Category> CloneCategoryListOrdered(
            List<DomainEntity.Category> categoryList, string orderBy, SearchOrder order)
        {
            var listClone = new List<DomainEntity.Category>(categoryList);
            var orderedEnumerable = (orderBy.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name)
                    .ThenBy(x => x.Id),
                ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name)
                    .ThenByDescending(x => x.Id),
                ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
                ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
                ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
                ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
                _ => listClone.OrderBy(x => x.Name)
                    .ThenBy(x => x.Id),
            };
            return orderedEnumerable.ToList();
        }
    }
}
