using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember
{
    public class CreateCastMember : ICreateCastMember
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICastMemberRepository _repository;

        public CreateCastMember(ICastMemberRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CastMemberModelOutput> Handle(CreateCastMemberInput request, CancellationToken cancellationToken)
        {
            var castMember = new DomainEntity.CastMember(request.Name, request.Type);
            await _repository.Insert(castMember, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return CastMemberModelOutput.FromCastMember(castMember);
        }
    }
}
