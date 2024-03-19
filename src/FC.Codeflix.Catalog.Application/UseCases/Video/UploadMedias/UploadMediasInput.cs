using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias
{
    public record UploadMediasInput(
        Guid Id,
        FileInput? VideoFile,
        FileInput? TraileFile
        ) : IRequest;

}
