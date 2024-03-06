using FC.Codeflix.Catalog.Domain.Enum;

namespace FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember
{
    public class UpdateCastMemberApiInput
    {
        public string Name { get; set; }
        public CastMemberType Type { get; set; }

        public UpdateCastMemberApiInput(string name, CastMemberType type)
        {
            Name = name;
            Type = type;
        }
    }
}
