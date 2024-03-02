using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers
{
    public class ListCastMemberInput : PaginatedListInput, IRequest<ListCastMembersOutput>
    {
        public ListCastMemberInput(
            int page, 
            int perPage, 
            string search, 
            string sort, 
            SearchOrder dir) 
            : base(page, perPage, search, sort, dir)
        {
        }
    }
}
