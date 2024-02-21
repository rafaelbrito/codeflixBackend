using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategory
{
    [CollectionDefinition (nameof(GetCategoryApiTestFixture))]
    public class GetCategoryApiTestFixtureCollection : ICollectionFixture<GetCategoryApiTestFixture>
    { }

    public class GetCategoryApiTestFixture: CategoryBaseFixture
    {
        
    }
}
