﻿using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo
{
    public record CreateVideoInput(
        string Title,
        string Description,
        int YearLauched,
        bool Opened,
        bool Published,
        int Duration,
        Rating Rating,
        IReadOnlyCollection<Guid>? CategoriesIds = null,
        IReadOnlyCollection<Guid>? GenresIds = null,
        IReadOnlyCollection<Guid>? CastMembersIds = null,
        FileInput? Thumb = null,
        FileInput? Banner = null,
        FileInput? ThumbHalf = null

        ) : IRequest<CreateVideoOutput>;

}
