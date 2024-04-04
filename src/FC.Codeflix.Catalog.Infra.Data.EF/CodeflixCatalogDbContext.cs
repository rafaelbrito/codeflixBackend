using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF.Congifurations;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF
{
    public class CodeflixCatalogDbContext : DbContext
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<GenresCategories> GenresCategories => Set<GenresCategories>();
        public DbSet<CastMember> CastMembers => Set<CastMember>();
        public DbSet<Video> Videos => Set<Video>();
        public DbSet<VideosCategories> VideosCategories => Set<VideosCategories>();
        public DbSet<VideosGenres> VideosGenres => Set<VideosGenres>();
        public DbSet<VideoCastMembers> VideosCastMembers => Set<VideoCastMembers>();




        public CodeflixCatalogDbContext(DbContextOptions<CodeflixCatalogDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new GenreConfiguration());
            builder.ApplyConfiguration(new GenresCategoriesConfiguration());
            builder.ApplyConfiguration(new CastMembersConfiguration());
            builder.ApplyConfiguration(new VideoConfiguration());
            builder.ApplyConfiguration(new VideosCategoriesConfiguration());
            builder.ApplyConfiguration(new VideosGenresConfiguration());
            builder.ApplyConfiguration(new VideosCastMembersConfiguration());
        }
    }
}
