using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers
{
    public class ListCastMembersOutput : PaginatedListOutput<CastMemberModelOutput>
    {
        public ListCastMembersOutput(
            int page,
            int perPage,
            int total,
            IReadOnlyList<CastMemberModelOutput> items)
            : base(page, perPage, total, items)
        { }

        public static ListCastMembersOutput FromSearchOutput(SearchOutput<DomainEntity.CastMember> searchOutput)
            => new ListCastMembersOutput(
                    searchOutput.CurrentPage,
                    searchOutput.PerPage,
                    searchOutput.Total,
                    searchOutput.Items
                        .Select(CastMemberModelOutput.FromCastMember).ToList()
                );
    }
}
