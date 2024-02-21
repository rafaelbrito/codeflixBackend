using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre
{
    public class CreateGenre : ICreateGenre
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;

        public CreateGenre(IGenreRepository genreRepository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
        {
            _genreRepository = genreRepository;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }

        public async Task<GenreModelOutput> Handle(CreateGenreInput request, CancellationToken cancellationToken)
        {
            var genre = new DomainEntity.Genre(request.Name, request.IsActive);
            if ((request.CategoriesIds?.Count ?? 0) > 0)
            {
                await ValidateCategoryIds(request, cancellationToken);
                request.CategoriesIds?.ForEach(genre.AddCategory);
            }
            await _genreRepository.Insert(genre, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return GenreModelOutput.FromGenre(genre);
        }

        private async Task ValidateCategoryIds(CreateGenreInput request, CancellationToken cancellationToken)
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
