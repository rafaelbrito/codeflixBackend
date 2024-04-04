using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Congifurations
{
    internal class VideosCastMembersConfiguration : IEntityTypeConfiguration<VideoCastMembers>
    {
        public void Configure(EntityTypeBuilder<VideoCastMembers> builder)
            => builder.HasKey(relation => new
            {
                relation.VideoId,
                relation.CastMembersId
            }
            );
    }
}
