using FC.Codeflix.Catalog.Application.Common;

namespace FC.Codeflix.Catalog.Api.ApiModels.Response
{
    public class ApiResponseList<TItemData> : ApiResponse<IReadOnlyList<TItemData>>
    {
        public ApiResonseListMeta Meta { get; private set; }

        public ApiResponseList(
            int currentPage,
            int perPage,
            int Total,
            IReadOnlyList<TItemData> data)
            : base(data)
        {
            Meta = new ApiResonseListMeta(currentPage, perPage, Total);
        }

        public ApiResponseList(
            PaginatedListOutput<TItemData> paginatedListOutput)
            : base(paginatedListOutput.Items)
        {
            Meta = new ApiResonseListMeta(
                paginatedListOutput.Page,
                paginatedListOutput.PerPage,
                paginatedListOutput.Total
                );
        }
    }
}
