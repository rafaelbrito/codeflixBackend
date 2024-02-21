namespace FC.Codeflix.Catalog.Api.ApiModels.Response
{
    public class ApiResonseListMeta
    {
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }

        public ApiResonseListMeta(int currentPage, int perPage, int total)
        {
            CurrentPage = currentPage;
            PerPage = perPage;
            Total = total;
        }
    }
}
