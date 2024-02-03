using FC.Codeflix.Catalog.UniTests.Application.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Category.DeleteCategory
{
    [CollectionDefinition(nameof(DeleteCategoryTestFixture))]
    public class DeleteCategoryTestFixtureCollection : ICollectionFixture<DeleteCategoryTestFixture>
    { }

    public class DeleteCategoryTestFixture : CategoryUseCasesBaseFixture
    { }
}
