using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo
{
    public record CreateVideoOutput(
        Guid Id,
        DateTime CreatedAt,
        string Title,
        bool Published,
        string Description,
        Rating Rating,
        int YearLauched,
        bool Opened,
        int Duration,
        IReadOnlyCollection<Guid> CategoriesIds,
        IReadOnlyCollection<Guid> GenresIds,
        IReadOnlyCollection<Guid> CastMembersIds,
        string? Thumb,
        string? Banner,
        string? ThumbHalf
         )
    {
        public static CreateVideoOutput FromVideo(Domain.Entity.Video video)
            => new CreateVideoOutput(
                 video.Id,
                 video.CreatedAt,
                 video.Title,
                 video.Published,
                 video.Description,
                 video.Rating,
                 video.YearLauched,
                 video.Opened,
                 video.Duration,
                 video.Categories,
                 video.Genres,
                 video.CastMembers,
                 video.Thumb?.Path,
                 video.Banner?.Path,
                 video.ThumbHalf?.Path);
    }
}
