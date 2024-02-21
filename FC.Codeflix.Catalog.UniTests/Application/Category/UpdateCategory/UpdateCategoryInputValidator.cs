using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentValidation;

namespace FC.Codeflix.Catalog.UniTests.Application.Category.UpdateCategory
{
    public class UpdateCategoryInputValidator : AbstractValidator<UpdateCategoryInput>
    {
        public UpdateCategoryInputValidator()
        => RuleFor(x => x.Id).NotEmpty();
    }
}
