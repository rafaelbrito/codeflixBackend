using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories
{
    public class CastMemberRepository : ICastMemberRepository
    {
        private readonly CodeflixCatalogDbContext _context;
        private DbSet<CastMember> _castMembers
            => _context.Set<CastMember>();

        public CastMemberRepository(CodeflixCatalogDbContext context)
           => _context = context;

        public async Task Delete(CastMember agreggate, CancellationToken cancellationToken)
            => await Task.FromResult(_castMembers.Remove(agreggate));

        public async Task<CastMember> Get(Guid id, CancellationToken cancellationToken)
        {
            var castMember = await _castMembers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            NotFoundException.ThrowIfNull(castMember, $"CastMember '{id}' not found.");
            return castMember!;
        }

        public async Task Insert(CastMember agreggate, CancellationToken cancellationToken)
                => await _castMembers.AddAsync(agreggate, cancellationToken);

        public async Task<SearchOutput<CastMember>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _castMembers.AsNoTracking();
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

        public async Task Update(CastMember agreggate, CancellationToken cancellationToken)
             => await Task.FromResult(_castMembers.Update(agreggate));

        private IQueryable<CastMember> AddOrderToQuery(IQueryable<CastMember> query, string orderProperty, SearchOrder order)
        {

            var orderedQuery = (orderProperty.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name)
                         .ThenBy(x => x.Id),
                ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name)
                         .ThenByDescending(x => x.Id),
                ("type", SearchOrder.Asc) => query.OrderBy(x => x.Type)
               .ThenBy(x => x.Type),
                ("type", SearchOrder.Desc) => query.OrderByDescending(x => x.Type)
                         .ThenByDescending(x => x.Type),
                ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
                ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
                ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
                ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
                _ => query.OrderBy(x => x.Name)
                        .ThenBy(x => x.Id),
            };
            return orderedQuery;
        }

        public async Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken)
             => await _castMembers.AsNoTracking()
                    .Where(castMember => ids.Contains(castMember.Id))
                    .Select(castMember => castMember.Id).ToListAsync();
    }
}
