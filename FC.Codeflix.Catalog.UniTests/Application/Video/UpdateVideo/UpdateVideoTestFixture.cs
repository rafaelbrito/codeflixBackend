using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.Codeflix.Catalog.UniTests.Common.Fixtures;
using Xunit;

namespace FC.Codeflix.Catalog.UniTests.Application.Video.UpdateVideo
{
    [CollectionDefinition(nameof(UpdateVideoTestFixture))]
    public class UpdateVideoTestFixtureCollection : ICollectionFixture<UpdateVideoTestFixture>
    { }

    public class UpdateVideoTestFixture : VideoBaseTestFixture
    {
        internal UpdateVideoInput CreateValidInput(
            Guid videoId, List<Guid>?
            genreIds = null,
            List<Guid>? categoriesIds = null,
            List<Guid>? castMembersIds = null,
            FileInput? thumb = null,
            FileInput? banner = null,
            FileInput? thumbHalf = null,
            FileInput? media = null,
            FileInput? trailer = null)
                => new UpdateVideoInput(
                   videoId,
                   GetValidVideoTitle(),
                   GetValidVideoDescription(),
                   GetValidYearLauched(),
                   GetRandomBoolean(),
                   GetRandomBoolean(),
                   GetValidVideoDuration(),
                   GetRandomRating(),
                   GenresIds: genreIds,
                   CategoriesIds: categoriesIds,
                   CastMembersIds: castMembersIds,
                   Thumb: thumb,
                   Banner: banner,
                   ThumbHalf: thumbHalf,
                   Media: media,
                   Trailer: trailer
                   );
    }
}
