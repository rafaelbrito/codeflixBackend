using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Model
{
    public class VideosCategories
    {
        public Guid VideoId { get; set; }
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
        public Video? Video { get; set; }

        public VideosCategories(Guid videoId, Guid categoryId)
        {
            VideoId = videoId;
            CategoryId = categoryId;
        }
    }
}
