using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre
{
    public class UpdateGenre : IUpdateGenre
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
        {
            _genreRepository = genreRepository;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }

        public async Task<GenreModelOutput> Handle(UpdateGenreInput request, CancellationToken cancellationToken)
        {
            var genre = await _genreRepository.Get(request.Id, cancellationToken);
            genre.Update(request.Name);
            if (request.IsActive is not null
                && request.IsActive != genre.IsActive)
            {
                if ((bool)request.IsActive) genre.Activate();
                else genre.Deactivate();
            }
            if (request.CategoriesIds is not null)
            {
                genre.RemoveAllCategory();
                if (request.CategoriesIds.Count > 0)
                {
                    await ValidateCategoryIds(request, cancellationToken);
                    request.CategoriesIds?.ForEach(genre.AddCategory);
                }
            }
            await _genreRepository.Update(genre, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return GenreModelOutput.FromGenre(genre);
        }

        private async Task ValidateCategoryIds(UpdateGenreInput request, CancellationToken cancellationToken)
        {
            var IdsInPersistence = await _categoryRepository
                .GetIdsListByIds(
                request.CategoriesIds!,
                cancellationToken
                );
            if (IdsInPersistence.Count < request.CategoriesIds!.Count)
            {
                var notFoundIds = request.CategoriesIds
                    .FindAll(x => !IdsInPersistence.Contains(x));
                var notFoudIdsAsString = String.Join(", ", notFoundIds);
                throw new RelatedAggregateException($"Related category id (or ids) not found: '{notFoudIdsAsString}'");
            }
        }
    }
}
