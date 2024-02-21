
using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory
{
    public class DeleteCategory : IDeleteCategory
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategory(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteCategoryInput request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.Get(request.Id, cancellationToken);
            await _categoryRepository.Delete(category, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
        }
    }
}
