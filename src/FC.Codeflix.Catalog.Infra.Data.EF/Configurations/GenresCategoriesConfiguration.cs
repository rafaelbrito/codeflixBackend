using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Congifurations
{
    internal class GenresCategoriesConfiguration : IEntityTypeConfiguration<GenresCategories>
    {
        public void Configure(EntityTypeBuilder<GenresCategories> builder)
            => builder.HasKey(relation => new
            {
                relation.CategoryId,
                relation.GenreId
            }
            );
    }
}
