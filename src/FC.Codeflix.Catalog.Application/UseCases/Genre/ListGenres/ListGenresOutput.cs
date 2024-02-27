using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenres
{
    public class ListGenresOutput : PaginatedListOutput<GenreModelOutput>
    {
        public ListGenresOutput(
            int page,
            int perPage,
            int total,
            IReadOnlyList<GenreModelOutput> items)

            : base(page, perPage, total, items)
        { }

        public static ListGenresOutput FromSearchOutput(SearchOutput<DomainEntity.Genre> searchOutput)
            => new ListGenresOutput(
                    searchOutput.CurrentPage,
                    searchOutput.PerPage,
                    searchOutput.Total,
                    searchOutput.Items
                        .Select(GenreModelOutput.FromGenre).ToList()
                );

        internal void FillWithCategoryName(IReadOnlyList<DomainEntity.Category> categories)
        {
            foreach (var item in Items)
                foreach (var categoryOutput in item.Categories)
                    categoryOutput.Name = categories?.FirstOrDefault(
                        category => category.Id == categoryOutput.Id)?.Name;
        }
    }
}

