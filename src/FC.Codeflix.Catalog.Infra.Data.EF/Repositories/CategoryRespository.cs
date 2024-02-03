using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories
{
    public class CategoryRespository : ICategoryRepository
    {
        private readonly CodeflixCatalogDbContext _context;
        private DbSet<Category> _categories
            => _context.Set<Category>();

        public CategoryRespository(CodeflixCatalogDbContext context)
            => _context = context;

        public Task Delete(Category agreggate, CancellationToken cancellationToken)
        => Task.FromResult(_categories.Remove(agreggate));

        public async Task<Category> Get(Guid id, CancellationToken cancellationToken)
        {
            var category = await _categories.AsNoTracking().FirstOrDefaultAsync(
                         x => x.Id == id,
                         cancellationToken
                         );
            NotFoundException.ThrowIfNull(category, $"Category '{id}' not found.");
            return category!;

        }

        public async Task Insert(Category agreggate,
            CancellationToken cancellationToken)
                => await _categories.AddAsync(agreggate, cancellationToken)
                  ;

        public async Task<SearchOutput<Category>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _categories.AsNoTracking();
            query = AddOrderToQuery(query, input.OrderBy, input.Order);
            if (!String.IsNullOrWhiteSpace(input.Search))
                query = query.Where(x => x.Name.Contains(input.Search));

            var total = await query.CountAsync();
            var items = await query
                .Skip(toSkip)
                .Take(input.PerPage)
                .ToListAsync();
            return new(input.Page, input.PerPage, total, items);
        }

        public Task Update(Category agreggate, CancellationToken cancellationToken)
        => Task.FromResult(_categories.Update(agreggate));

        private IQueryable<Category> AddOrderToQuery(IQueryable<Category> query, string orderProperty, SearchOrder order)
         => (orderProperty.ToLower(), order) switch
         {
             ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name),
             ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name),
             ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
             ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
             ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
             ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
             _ => query.OrderBy(x => x.Name),
         };
    }
}
