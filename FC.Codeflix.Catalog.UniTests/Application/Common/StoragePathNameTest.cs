using FC.Codeflix.Catalog.Application.Common;
using FluentAssertions;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Common
{
    public class StoragePathNameTest
    {
        [Fact(DisplayName = (nameof(CreateStoragePathName)))]
        [Trait("Application", "StorageName - Common")]
        public static void CreateStoragePathName()
        {
            var exampleId = Guid.NewGuid();
            var exampleExtension = "mp4";
            var propertyName = "Video";

            var name = StorageFileName.Create(exampleId, propertyName, exampleExtension);
            name.Should().Be($"{exampleId}-{propertyName.ToLower()}.{exampleExtension}");
        }
    }
}
