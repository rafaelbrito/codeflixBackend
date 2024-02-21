using FC.Codeflix.Catalog.UniTests.Application.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Category.GetCategory
{
    [CollectionDefinition(nameof(GetCategoryTestFixture))]
    public class GetCategoryTestFixtureCollection : ICollectionFixture<GetCategoryTestFixture>
    { }

    public class GetCategoryTestFixture : CategoryUseCasesBaseFixture
    { }
}
