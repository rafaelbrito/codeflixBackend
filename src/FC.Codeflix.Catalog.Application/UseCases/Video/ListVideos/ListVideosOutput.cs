using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideos
{
    public class ListVideosOutput : PaginatedListOutput<VideoModelOutput>
    {
        public ListVideosOutput(
            int page,
            int perPage,
            int total,
            IReadOnlyList<VideoModelOutput> items)
            : base(page, perPage, total, items)
        { }

        public static ListVideosOutput FromSearchOutput(SearchOutput<Domain.Entity.Video> searchOutput,
            IReadOnlyList<DomainEntity.Category>? categories, IReadOnlyList<DomainEntity.Genre>? genres)
            => new ListVideosOutput(
                    searchOutput.CurrentPage,
                    searchOutput.PerPage,
                    searchOutput.Total,
                    searchOutput.Items
                        .Select(item => VideoModelOutput.FromVideo(item, categories, genres)).ToList()
                );

        public static ListVideosOutput FromSearchOutput(SearchOutput<Domain.Entity.Video> searchOutput)
            => new ListVideosOutput(
                    searchOutput.CurrentPage,
                    searchOutput.PerPage,
                    searchOutput.Total,
                    searchOutput.Items
                        .Select(VideoModelOutput.FromVideo).ToList()
                );
    }
}
