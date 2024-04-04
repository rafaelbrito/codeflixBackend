using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Congifurations
{
    internal class VideosCategoriesConfiguration : IEntityTypeConfiguration<VideosCategories>
    {
        public void Configure(EntityTypeBuilder<VideosCategories> builder)
            => builder.HasKey(relation => new
            {
                relation.VideoId,
                relation.CategoryId
            }
            );
    }
}
