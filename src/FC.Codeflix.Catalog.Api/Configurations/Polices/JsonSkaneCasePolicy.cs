using FC.Codeflix.Catalog.Api.Extensions.String;
using System.Text.Json;

namespace FC.Codeflix.Catalog.Api.Configurations.Polices
{
    public class JsonSkaneCasePolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
            => name.ToSnakeCase();
    }
}
