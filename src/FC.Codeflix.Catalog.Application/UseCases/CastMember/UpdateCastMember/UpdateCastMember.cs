using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember
{
    public class UpdateCastMember : IUpdateCastMember
    {
        private readonly ICastMemberRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCastMember(ICastMemberRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CastMemberModelOutput> Handle(UpdateCastMemberInput request, CancellationToken cancellationToken)
        {
            var castMember = await _repository.Get(request.Id, cancellationToken);
            castMember.Update(request.Name, request.Type);
            await _repository.Update(castMember, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return CastMemberModelOutput.FromCastMember(castMember);
        }
    }
}
