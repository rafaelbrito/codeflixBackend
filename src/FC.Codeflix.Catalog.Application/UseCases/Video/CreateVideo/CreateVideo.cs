using FC.Codeflix.Catalog.Application.Inferfaces;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo
{
    public class CreateVideo : ICreateVideo
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ICastMemberRepository _castMemberRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public CreateVideo(
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

        public async Task<CreateVideoOutput> Handle(CreateVideoInput request, CancellationToken cancellationToken)
        {
            var video = new DomainEntity.Video(
                request.Title,
                request.Description,
                request.YearLauched,
                request.Opened,
                request.Published,
                request.Duration,
                request.Rating
                );
            var validationHandler = new NotificationValidationHandler();
            video.Validate(validationHandler);
            if (validationHandler.HasErrors())
                throw new EntityValidationException(
                    "There are validation errors",
                    validationHandler.Errors);

            await ValidateAddRelations(request, video, cancellationToken);
            try
            {
                await UploadImagesMedia(request, video, cancellationToken);

                await _videoRepository.Insert(video, cancellationToken);
                await _unitOfWork.Commit(cancellationToken);
                return CreateVideoOutput.FromVideo(video);
            }
            catch (Exception)
            {
                await ClearStorage(video, cancellationToken);

                throw;
            }

        }

        private async Task ClearStorage(DomainEntity.Video video, CancellationToken cancellationToken)
        {
            if (video.Thumb is not null)
                await _storageService.Delete(video.Thumb.Path, cancellationToken);
            if (video.ThumbHalf is not null)
                await _storageService.Delete(video.ThumbHalf.Path, cancellationToken);
            if (video.Banner is not null)
                await _storageService.Delete(video.Banner.Path, cancellationToken);
        }

        private async Task UploadImagesMedia(CreateVideoInput request, DomainEntity.Video video, CancellationToken cancellationToken)
        {
            if (request.Thumb is not null)
            {
                var thumbUrl = await _storageService.Upload(
                    $"{video.Id}-thumb.{request.Thumb.Extension}",
                    request.Thumb.FileStream,
                    cancellationToken);
                video.UpdateThumb(thumbUrl);
            }
            if (request.Banner is not null)
            {
                var bannerUrl = await _storageService.Upload(
                    $"{video.Id}-banner.{request.Banner.Extension}",
                    request.Banner.FileStream,
                    cancellationToken);
                video.UpdateBanner(bannerUrl);
            }

            if (request.ThumbHalf is not null)
            {
                var thumbHalfUrl = await _storageService.Upload(
                    $"{video.Id}-thumbhalf.{request.ThumbHalf.Extension}",
                    request.ThumbHalf.FileStream,
                    cancellationToken);
                video.UpdateThumbHalf(thumbHalfUrl);
            }
        }

        private async Task ValidateAddRelations(CreateVideoInput request, DomainEntity.Video video, CancellationToken cancellationToken)
        {
            if ((request.CastMembersIds?.Count ?? 0) > 0)
            {
                await ValidateCastMembersIds(request, cancellationToken);
                request.CastMembersIds!.ToList().ForEach(video.AddCastMember);
            }

            if ((request.GenresIds?.Count ?? 0) > 0)
            {
                await ValidateGenresIds(request, cancellationToken);
                request.GenresIds!.ToList().ForEach(video.AddGenre);
            }

            if ((request.CategoriesIds?.Count ?? 0) > 0)
            {
                await ValidateCategoriesIds(request, cancellationToken);
                request.CategoriesIds!.ToList().ForEach(video.AddCategory);
            }
        }

        private async Task ValidateCategoriesIds(CreateVideoInput request, CancellationToken cancellationToken)
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

        private async Task ValidateGenresIds(CreateVideoInput request, CancellationToken cancellationToken)
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

        private async Task ValidateCastMembersIds(CreateVideoInput request, CancellationToken cancellationToken)
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
    }
}
