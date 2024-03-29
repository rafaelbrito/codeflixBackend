using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos
{
    public class ListVideos : IListVideos
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGenreRepository _genreRepository;

        public ListVideos(
            IVideoRepository videoRepository,
            ICategoryRepository categoryRepository,
            IGenreRepository genreRepository)
        {
            _videoRepository = videoRepository;
            _categoryRepository = categoryRepository;
            _genreRepository = genreRepository;
        }

        public async Task<ListVideosOutput> Handle(ListVideosInput input, CancellationToken cancellationToken)
        {
            var searchOutput = await _videoRepository.Search(input.ToSearchInput(), cancellationToken);
            var categories = await ListIdsCategories(searchOutput, cancellationToken);
            var genres = await ListIdsGenres(searchOutput, cancellationToken);

            return ListVideosOutput.FromSearchOutput(searchOutput, categories, genres);
        }

        private async Task<IReadOnlyList<DomainEntity.Genre>?> ListIdsGenres(SearchOutput<DomainEntity.Video> searchOutput, CancellationToken cancellationToken)
        {
            IReadOnlyList<DomainEntity.Genre>? genres = null;
            var relatedGenresIds = searchOutput.Items.SelectMany(video => video.Genres).Distinct().ToList();
            if (relatedGenresIds is not null && relatedGenresIds.Count > 0)
                genres = await _genreRepository.GetListByIds(relatedGenresIds, cancellationToken);
            return genres;
        }

        private async Task<IReadOnlyList<DomainEntity.Category>?> ListIdsCategories(SearchOutput<DomainEntity.Video> searchOutput, CancellationToken cancellationToken)
        {
            IReadOnlyList<DomainEntity.Category>? categories = null;
            var relatedCategoriesIds = searchOutput.Items.SelectMany(video => video.Categories).Distinct().ToList();
            if (relatedCategoriesIds is not null && relatedCategoriesIds.Count > 0)
                categories = await _categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);
            return categories;
        }
    }
}
