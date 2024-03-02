using FC.Codeflix.Catalog.UniTests.Domain.Entity.CastMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.CastMember.ListCastMember
{
    [CollectionDefinition(nameof(ListCastMemberTestFixture))]
    public class ListCastMemberTestFixtureCollection : ICollectionFixture<ListCastMemberTestFixture>
    { }
    public class ListCastMemberTestFixture:CastMemberTestFixture
    {
    }
}
