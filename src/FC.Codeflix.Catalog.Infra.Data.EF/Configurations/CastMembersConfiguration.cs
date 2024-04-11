using FC.Codeflix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Congifurations
{
    internal class CastMembersConfiguration : IEntityTypeConfiguration<CastMember>
    {
        public void Configure(EntityTypeBuilder<CastMember> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(255)
                ;
            builder.Ignore(CastMember => CastMember.Events);

        }
    }
}
