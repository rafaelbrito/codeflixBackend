using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly CodeflixCatalogDbContext _context;
        private DbSet<Video> _videos => _context.Set<Video>();
        private DbSet<Media> _medias => _context.Set<Media>();
        private DbSet<VideosCategories> _videosCategories => _context.Set<VideosCategories>();
        private DbSet<VideosGenres> _videosGenres => _context.Set<VideosGenres>();
        private DbSet<VideoCastMembers> _videoCastMembers => _context.Set<VideoCastMembers>();

        public VideoRepository(CodeflixCatalogDbContext context)
            => _context = context;

        public Task Delete(Video video, CancellationToken cancellationToken)
        {
            RemoveRelations(video);
            RemoveMediasAndTrailer(video);
            _videos.Remove(video);
            return Task.CompletedTask;
        }

        public async Task<Video> Get(Guid id, CancellationToken cancellationToken)
        {
            var video = await _videos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            NotFoundException.ThrowIfNull(video, $"Video '{id}' not found.");
            await SelectRelations(video!, cancellationToken);
            return video!;
        }

        public async Task Insert(Video video, CancellationToken cancellationToken)
        {
            await _videos.AddAsync(video, cancellationToken);
            await AddRelations(video);
        }

        public async Task<SearchOutput<Video>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _videos.AsNoTracking();
            if (!String.IsNullOrWhiteSpace(input.Search))
                query = query.Where(video => video.Title.Contains(input.Search));
            query = AddOrderToQuery(query, input);


            var total = await query.CountAsync();

            var videos = await query
                .Skip(toSkip).Take(input.PerPage).ToListAsync();

            var videosIds = videos.Select(video => video.Id).ToList();
            await AddCategoriesToVideos(videos, videosIds);
            await AddGenresToVideos(videos, videosIds);
            await AddCastMembersToVideos(videos, videosIds);

            return new SearchOutput<Video>(
                input.Page,
                input.PerPage,
                total,
                videos
                );
        }


        public async Task Update(Video video, CancellationToken cancellationToken)
        {
            RemoveRelations(video);
            await AddRelations(video);
            _videos.Update(video);
        }

        private IQueryable<Video> AddOrderToQuery(IQueryable<Video> query, SearchInput input)
            => input switch
            {
                { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "title"
                    => query.OrderBy(video => video.Title).ThenBy(video => video.Id),
                { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "title"
                    => query.OrderByDescending(video => video.Title).ThenByDescending(video => video.Id),
                { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "description"
                    => query.OrderBy(video => video.Description).ThenBy(video => video.Id),
                { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "description"
                    => query.OrderByDescending(video => video.Description).ThenByDescending(video => video.Opened),
                { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "id"
                    => query.OrderBy(video => video.Id),
                { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "id"
                    => query.OrderByDescending(video => video.Id),
                { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "createdat"
                    => query.OrderBy(video => video.CreatedAt).ThenBy(video => video.Id),
                { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "createdat"
                    => query.OrderByDescending(video => video.CreatedAt).ThenByDescending(video => video.Id),
                _ => query.OrderBy(video => video.Title).ThenBy(video => video.Id),
            };

        private async Task AddCategoriesToVideos(List<Video> videos, List<Guid> videosIds)
        {
            var relationsCategories = await _videosCategories.Where(
               relation => videosIds.Contains(relation.VideoId)
               ).ToListAsync();
            var relationsByCategoryIdGroup = relationsCategories.GroupBy(x => x.VideoId).ToList();
            relationsByCategoryIdGroup.ForEach(relationGroup =>
            {
                var video = videos.Find(video => video.Id == relationGroup.Key);
                if (video is null) return;
                relationGroup.ToList()
                    .ForEach(relation => video.AddCategory(relation.CategoryId));
            });
        }

        private async Task AddGenresToVideos(List<Video> videos, List<Guid> videosIds)
        {
            var relationsGenres = await _videosGenres.Where(
                            relation => videosIds.Contains(relation.VideoId)
                            ).ToListAsync();
            var relationsByVideoIdGroup = relationsGenres.GroupBy(x => x.VideoId).ToList();
            relationsByVideoIdGroup.ForEach(relationGroup =>
            {
                var video = videos.Find(video => video.Id == relationGroup.Key);
                if (video is null) return;
                relationGroup.ToList()
                    .ForEach(relation => video.AddGenre(relation.GenreId));
            });
        }

        private async Task AddCastMembersToVideos(List<Video> videos, List<Guid> videosIds)
        {
            var relationsCastMembers = await _videoCastMembers
                .Where(relation => videosIds.Contains(relation.VideoId))
                .ToListAsync();
            var relationsByCastMemberIdGroup = relationsCastMembers.GroupBy(x => x.VideoId).ToList();
            relationsByCastMemberIdGroup.ForEach(relationGroup =>
            {
                var video = videos.Find(video => video.Id == relationGroup.Key);
                if (video is null) return;
                relationGroup.ToList()
                    .ForEach(relation => video.AddCastMember(relation.CastMembersId));
            });
        }

        private async Task SelectRelations(Video video, CancellationToken cancellationToken)
        {
            var categoryId = await _videosCategories
                .Where(c => c.VideoId == video!.Id)
                .Select(c => c.CategoryId)
                .ToListAsync(cancellationToken);
            categoryId.ForEach(video!.AddCategory);

            var genreId = await _videosGenres
                .Where(c => c.VideoId == video!.Id)
                .Select(c => c.GenreId)
                .ToListAsync(cancellationToken);
            genreId.ForEach(video!.AddGenre);

            var castMembersId = await _videoCastMembers
                .Where(c => c.VideoId == video!.Id)
                .Select(c => c.CastMembersId)
                .ToListAsync(cancellationToken);
            castMembersId.ForEach(video!.AddCastMember);
        }

        private void RemoveMediasAndTrailer(Video video)
        {
            if (video.Trailer is not null)
                _medias.Remove(video.Trailer);
            if (video.Media is not null)
                _medias.Remove(video.Media);
        }

        private void RemoveRelations(Video video)
        {
            _videosGenres.RemoveRange(_videosGenres
                 .Where(x => x.VideoId == video.Id));
            _videosCategories.RemoveRange(_videosCategories
                .Where(x => x.VideoId == video.Id));
            _videoCastMembers.RemoveRange(_videoCastMembers
                .Where(x => x.VideoId == video.Id));
        }

        private async Task AddRelations(Video video)
        {
            if (video.Categories.Count > 0)
            {
                var relations = video.Categories
                    .Select(categoryId => new VideosCategories(
                        video.Id,
                        categoryId
                        ));
                await _videosCategories.AddRangeAsync(relations);
            }

            if (video.Genres.Count > 0)
            {
                var relations = video.Genres
                    .Select(genreId => new VideosGenres(
                        video.Id,
                        genreId
                        ));
                await _videosGenres.AddRangeAsync(relations);
            }

            if (video.CastMembers.Count > 0)
            {
                var relations = video.CastMembers
                    .Select(castMemberId => new VideoCastMembers(
                        video.Id,
                        castMemberId
                        ));
                await _videoCastMembers.AddRangeAsync(relations);
            }
        }
    }
}
