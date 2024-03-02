using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMembers
{
    public interface IListCastMembers:IRequestHandler<ListCastMemberInput, ListCastMembersOutput>
    {
    }
}
