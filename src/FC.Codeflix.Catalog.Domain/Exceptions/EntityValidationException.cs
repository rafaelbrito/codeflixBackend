using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Exceptions
{
    public class EntityValidationException : Exception
    {
        public IReadOnlyCollection<ValidationError>? Erros { get; }
        public EntityValidationException(
            string? message,
            IReadOnlyCollection<ValidationError>? erros = null)
            : base(message)
            => Erros = erros;
    }
}
