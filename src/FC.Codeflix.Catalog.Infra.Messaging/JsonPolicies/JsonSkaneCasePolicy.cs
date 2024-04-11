﻿using FC.Codeflix.Catalog.Infra.Messaging.Extensions;
using System.Text.Json;

namespace FC.Codeflix.Catalog.Infra.Messaging.JsonPolicies
{
    public class JsonSkaneCasePolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
            => name.ToSnakeCase();
    }
}
