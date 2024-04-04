using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Model
{
    public class VideosGenres
    {
        public Guid VideoId { get; set; }
        public Guid GenreId { get; set; }
        public Genre? Genre { get; set; }
        public Video? Video { get; set; }

        public VideosGenres(Guid videoId, Guid genreId)
        {
            VideoId = videoId;
            GenreId = genreId;
        }
    }
}
