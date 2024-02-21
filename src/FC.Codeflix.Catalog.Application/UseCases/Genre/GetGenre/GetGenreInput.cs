using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre
{
    public class GetGenreInput:IRequest<GenreModelOutput>
    {
        public Guid Id { get; set; }

        public GetGenreInput(Guid id) => Id = id;
    }
}
