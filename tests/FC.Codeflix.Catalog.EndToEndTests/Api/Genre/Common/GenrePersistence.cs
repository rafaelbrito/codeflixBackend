using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Model;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common
{
    public class GenrePersistence
    {
        private readonly CodeflixCatalogDbContext _context;

        public GenrePersistence(CodeflixCatalogDbContext context)
             => _context = context;

        public async Task InsertList(List<DomainEntity.Genre> genres, List<GenresCategories>? relation = null)
        {
            await _context.AddRangeAsync(genres);
            if ( relation != null ) await _context.AddRangeAsync(relation);
            await _context.SaveChangesAsync();
        }

        public async Task<DomainEntity.Genre?> GetById(Guid id)
            => await _context.Genres.AsNoTracking().FirstOrDefaultAsync(genre => genre.Id == id);

        public async Task<List<GenresCategories>> GetGenresCategoriesRelationsByGenreId(Guid id)
             => await _context.GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == id)
            .ToListAsync();
    }
}
