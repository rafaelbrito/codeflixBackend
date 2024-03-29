using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Extensions;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.Common
{
    public record VideoModelOutput(
        Guid Id,
        DateTime CreatedAt,
        string Title,
        bool Published,
        string Description,
        string Rating,
        int YearLauched,
        bool Opened,
        int Duration,
        IReadOnlyCollection<VideoModelOutputRelatedAggregate> Categories,
        IReadOnlyCollection<VideoModelOutputRelatedAggregate> Genres,
        IReadOnlyCollection<VideoModelOutputRelatedAggregate> CastMembers,
        string? ThumbFileUrl,
        string? BannerFileUrl,
        string? ThumbHalfFileUrl,
        string? VideoFileUrl,
        string? TrailerFileUrl
         )
    {
        public static VideoModelOutput FromVideo(DomainEntity.Video video)
            => new VideoModelOutput(
                 video.Id,
                 video.CreatedAt,
                 video.Title,
                 video.Published,
                 video.Description,
                 video.Rating.ToStringSignal(),
                 video.YearLauched,
                 video.Opened,
                 video.Duration,
                 video.Categories.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
                 video.Genres.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
                 video.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
                 video.Thumb?.Path,
                 video.Banner?.Path,
                 video.ThumbHalf?.Path,
                 video.Media?.FilePath,
                 video.Trailer?.FilePath);

        public static VideoModelOutput FromVideo(
            DomainEntity.Video video,
            IReadOnlyList<DomainEntity.Category>? categories = null,
            IReadOnlyList<DomainEntity.Genre>? genres = null)
            => new VideoModelOutput(
                 video.Id,
                 video.CreatedAt,
                 video.Title,
                 video.Published,
                 video.Description,
                 video.Rating.ToStringSignal(),
                 video.YearLauched,
                 video.Opened,
                 video.Duration,
                 video.Categories.Select(id => new VideoModelOutputRelatedAggregate(id, categories?
                     .FirstOrDefault(category => category.Id == id)?.Name)).ToList(),
                 video.Genres.Select(id => new VideoModelOutputRelatedAggregate(id, genres?.FirstOrDefault(genres => genres.Id == id)?.Name)).ToList(),
                 video.CastMembers.Select(id => new VideoModelOutputRelatedAggregate(id)).ToList(),
                 video.Thumb?.Path,
                 video.Banner?.Path,
                 video.ThumbHalf?.Path,
                 video.Media?.FilePath,
                 video.Trailer?.FilePath);
    }

    public record VideoModelOutputRelatedAggregate(Guid Id, string? Name = null);
}

