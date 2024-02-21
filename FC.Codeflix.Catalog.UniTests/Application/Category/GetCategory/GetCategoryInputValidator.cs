using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FluentValidation;

namespace FC.Codeflix.Catalog.UniTests.Application.Category.GetCategory
{
    public class GetCategoryInputValidator : AbstractValidator<GetCategoryInput>
    {
        public GetCategoryInputValidator()
          => RuleFor(x => x.Id).NotEmpty();
    }
}
