namespace FC.Codeflix.Catalog.Api.ApiModels.Category
{
    public class UpdateGenreApiInput
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public List<Guid>? CategoriesIds { get; set; }

        public UpdateGenreApiInput(string name, bool? isActive, List<Guid>? categoriesIds = null)
        {
            Name = name;
            IsActive = isActive;
            CategoriesIds = categoriesIds;
        }
    }
}
