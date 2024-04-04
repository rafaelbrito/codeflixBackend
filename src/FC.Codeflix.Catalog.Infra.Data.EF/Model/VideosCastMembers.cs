using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Model
{
    public class VideoCastMembers
    {
        public Guid CastMembersId { get; set; }
        public Guid VideoId { get; set; }
        public CastMember? CastMember { get; set; }
        public Video? Video { get; set; }

        public VideoCastMembers(Guid videoId, Guid castMembersId)
        {
            VideoId = videoId;
            CastMembersId = castMembersId;
        }
    }
}
