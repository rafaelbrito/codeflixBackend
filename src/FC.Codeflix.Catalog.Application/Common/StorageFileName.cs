namespace FC.Codeflix.Catalog.Application.Common
{
    public static class StorageFileName
    {
        public static string Create(Guid id, string propetyName, string extension)
            => $"{id}-{propetyName.ToLower()}.{extension.Replace(".","")}";
    }
}
