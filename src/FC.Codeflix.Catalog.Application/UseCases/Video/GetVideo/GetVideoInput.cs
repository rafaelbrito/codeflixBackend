using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo
{
    public record GetVideoInput(Guid VideoId) : IRequest<VideoModelOutput>
    {
    }
}
