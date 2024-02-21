using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre
{
    public class DeleteGenreInput : IRequest
    {
        public Guid Id { get; set; }

        public DeleteGenreInput(Guid id) => Id = id;

    }
}
