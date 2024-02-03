using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.Domain.Validation
{
    public static class DomainValidation
    {
        public static void NotNull(object? target, string fieldName)
        {
            if (target is null)
                throw new EntityValidationException($"{fieldName} should not be null");
        }

        public static void NotNullOrEmpty(string? target, string fieldName)
        {
            if (String.IsNullOrWhiteSpace(target))
                throw new EntityValidationException($"{fieldName} should not be null or empty");
        }

        public static void MinLength(string target, int minLegth, string fieldName)
        {
            if (target.Length < minLegth)
                throw new EntityValidationException($"{fieldName} should be at least " +
                    $"{minLegth} characters long");
        }

        public static void MaxLength(string target, int maxLegth, string fieldName)
        {
            if (target.Length > maxLegth)
                throw new EntityValidationException($"{fieldName} should be least or equal " +
                    $"{maxLegth} characters long");
        }
    }
}
