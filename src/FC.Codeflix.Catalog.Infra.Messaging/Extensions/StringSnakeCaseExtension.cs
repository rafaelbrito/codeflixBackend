﻿using Newtonsoft.Json.Serialization;

namespace FC.Codeflix.Catalog.Infra.Messaging.Extensions
{
    public static class StringSnakeCaseExtension
    {
        private readonly static NamingStrategy _snakeCasaNamingStrategy =
            new SnakeCaseNamingStrategy();

        public static string ToSnakeCase(this string stringToConvert)
        {
            ArgumentNullException.ThrowIfNull(stringToConvert, nameof(stringToConvert));
            return _snakeCasaNamingStrategy.GetPropertyName(stringToConvert, false);
        }
    }
}
