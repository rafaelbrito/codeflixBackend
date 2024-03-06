using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers
{
    public class ListCastMembersInput : PaginatedListInput, IRequest<ListCastMembersOutput>
    {
        public ListCastMembersInput(
            int page = 1,
            int perPage = 15,
            string search = "",
            string sort = "",
            SearchOrder dir = SearchOrder.Asc)
            : base(page, perPage, search, sort, dir)
        { }
    }
}
