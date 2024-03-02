using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember
{
    public interface ICreateCastMember:IRequestHandler<CreateCastMemberInput, CastMemberModelOutput>
    {
    }
}
