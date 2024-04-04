using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo
{
    public class UpdateVideo : IUpdateVideo
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ICastMemberRepository _castMemberRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public UpdateVideo(
            IVideoRepository videoRepository,
            ICategoryRepository categoryRepository,
            IGenreRepository genreRepository,
            ICastMemberRepository castMemberRepository,
            IUnitOfWork unitOfWork,
            IStorageService storageService)
        {
            _videoRepository = videoRepository;
            _categoryRepository = categoryRepository;
            _genreRepository = genreRepository;
            _castMemberRepository = castMemberRepository;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }

        public async Task<VideoModelOutput> Handle(UpdateVideoInput input, CancellationToken cancellationToken)
        {

            var video = await _videoRepository.Get(input.Id, cancellationToken);
            video.Update(input.Title, input.Description, input.YearLauched, input.Opened, input.Published, input.Duration, input.Rating);
            var validationHandler = new NotificationValidationHandler();
            video.Validate(validationHandler);
            if (validationHandler.HasErrors())
                throw new EntityValidationException(
                    "There are validation errors",
                    validationHandler.Errors);

            await ValidateAddRelations(input, video, cancellationToken);

            await UploadImagesMedia(input, video, cancellationToken);

            await _videoRepository.Update(video, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return VideoModelOutput.FromVideo(video);
        }

        private async Task ValidateAddRelations(UpdateVideoInput request, DomainEntity.Video video, CancellationToken cancellationToken)
        {

            if (request.GenresIds is not null)
            {
                video.RemoveAllGenres();
                if (request.GenresIds.Count > 0)
                {
                    await ValidateGenresIds(request, cancellationToken);
                    request.GenresIds!.ToList().ForEach(video.AddGenre);
                }
            }
            if (request.CategoriesIds is not null)
            {
                video.RemoveAllCategory();
                if (request.CategoriesIds.Count > 0)
                {
                    await ValidateCategoriesIds(request, cancellationToken);
                    request.CategoriesIds!.ToList().ForEach(video.AddCategory);
                }
            }
            if (request.CastMembersIds is not null)
            {
                video.RemoveAllCastMember();
                if (request.CastMembersIds.Count > 0)
                {
                    await ValidateCastMembersIds(request, cancellationToken);
                    request.CastMembersIds!.ToList().ForEach(video.AddCastMember);
                }
            }
        }

        private async Task ValidateGenresIds(UpdateVideoInput request, CancellationToken cancellationToken)
        {
            var persistenceIds = await _genreRepository.GetIdsListByIds(
                  request.GenresIds!.ToList(), cancellationToken);
            if (persistenceIds.Count < request.GenresIds!.Count)
            {
                var notFoudIds = request.GenresIds!
                    .ToList()
                    .FindAll(x => !persistenceIds.Contains(x));
                throw new RelatedAggregateException(
                    $"Related genres id (or ids) not found: '{string.Join(", ", notFoudIds)}'");
            }
        }

        private async Task ValidateCategoriesIds(UpdateVideoInput request, CancellationToken cancellationToken)
        {
            var persistenceIds = await _categoryRepository.GetIdsListByIds(
                  request.CategoriesIds!.ToList(), cancellationToken);
            if (persistenceIds.Count < request.CategoriesIds!.Count)
            {
                var notFoudIds = request.CategoriesIds!
                    .ToList()
                    .FindAll(x => !persistenceIds.Contains(x));
                throw new RelatedAggregateException(
                    $"Related category id (or ids) not found: '{string.Join(", ", notFoudIds)}'");
            }
        }

        private async Task ValidateCastMembersIds(UpdateVideoInput request, CancellationToken cancellationToken)
        {
            var persistenceIds = await _castMemberRepository.GetIdsListByIds(
                  request.CastMembersIds!.ToList(), cancellationToken);
            if (persistenceIds.Count < request.CastMembersIds!.Count)
            {
                var notFoudIds = request.CastMembersIds!
                    .ToList()
                    .FindAll(x => !persistenceIds.Contains(x));
                throw new RelatedAggregateException(
                    $"Related castmembers id (or ids) not found: '{string.Join(", ", notFoudIds)}'");
            }
        }

        private async Task UploadImagesMedia(UpdateVideoInput input, DomainEntity.Video video, CancellationToken cancellationToken)
        {
            if (input.Thumb is not null)
            {
                var fileName = StorageFileName.Create(video.Id, nameof(input.Thumb), input.Thumb!.Extension);
                var thumbUrl = await _storageService.Upload(
                    fileName,
                    input.Thumb.FileStream,
                    input.Thumb.ContentType,
                    cancellationToken);
                video.UpdateThumb(thumbUrl);
            }
            if (input.Banner is not null)
            {
                var fileName = StorageFileName.Create(video.Id, nameof(input.Banner), input.Banner!.Extension);
                var bannerUrl = await _storageService.Upload(
                    fileName,
                    input.Banner.FileStream,
                    input.Banner.ContentType,
                    cancellationToken);
                video.UpdateBanner(bannerUrl);
            }

            if (input.ThumbHalf is not null)
            {
                var fileName = StorageFileName.Create(video.Id, nameof(input.ThumbHalf), input.ThumbHalf!.Extension);
                var thumbHalfUrl = await _storageService.Upload(
                    fileName,
                    input.ThumbHalf.FileStream,
                    input.ThumbHalf.ContentType,
                    cancellationToken);
                video.UpdateThumbHalf(thumbHalfUrl);
            }
        }
    }
}
