﻿using FC.Codeflix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Congifurations
{
    internal class VideoConfiguration : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.HasKey(video => video.Id);
            builder.Navigation(x => x.Media).AutoInclude();
            builder.Navigation(x => x.Trailer).AutoInclude();
            builder.Property(video => video.Id)
                .ValueGeneratedNever();
            builder.Property(video => video.Title)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(c => c.Description)
                .HasMaxLength(4_000);
            builder.OwnsOne(video => video.Thumb, thumb =>
                 thumb.Property(image => image.Path).HasColumnName("ThumbPath")
            );
            builder.OwnsOne(video => video.ThumbHalf, thumbHalf =>
                thumbHalf.Property(image => image.Path).HasColumnName("ThumbHalfPath")
           );
            builder.OwnsOne(video => video.Banner, banner =>
                banner.Property(image => image.Path).HasColumnName("BannerPath")
           );
            builder.HasOne(x => x.Media).WithOne().HasForeignKey<Media>();
            builder.HasOne(x => x.Trailer).WithOne().HasForeignKey<Media>();
            builder.Ignore(video => video.Events);

        }
    }
}
