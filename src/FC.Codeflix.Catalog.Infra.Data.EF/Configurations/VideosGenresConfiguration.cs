using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Congifurations
{
    internal class VideosGenresConfiguration : IEntityTypeConfiguration<VideosGenres>
    {
        public void Configure(EntityTypeBuilder<VideosGenres> builder)
            => builder.HasKey(relation => new
            {
                relation.VideoId,
                relation.GenreId
            }
            );
    }
}
