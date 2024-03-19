using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias
{
    public class UploadMedias : IUploadMedias
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public UploadMedias(
            IVideoRepository videoRepository,
            IUnitOfWork unitOfWork,
            IStorageService storageService
            )
        {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }

        public async Task Handle(UploadMediasInput input, CancellationToken cancellationToken)
        {
            var video = await _videoRepository.Get(input.Id, cancellationToken);
            try
            {
                await UploadVideo(input, video, cancellationToken);
                await UploadTrailer(input, video, cancellationToken);
                await _videoRepository.Update(video, cancellationToken);
                await _unitOfWork.Commit(cancellationToken);
            }
            catch (Exception)
            {
                await ClearStorage(input, video, cancellationToken);
                throw;
            }
        }

        private async Task ClearStorage(UploadMediasInput input, Domain.Entity.Video video, CancellationToken cancellationToken)
        {
            if (input.VideoFile is not null && video.Media is not null)
                await _storageService.Delete(video.Media.FilePath, cancellationToken);
            if (input.TraileFile is not null && video.Trailer is not null)
                await _storageService.Delete(video.Trailer.FilePath, cancellationToken);
        }

        private async Task UploadTrailer(UploadMediasInput input, Domain.Entity.Video video, CancellationToken cancellationToken)
        {
            if (input.TraileFile is not null)
            {
                var fileName = StorageFileName.Create(input.Id, nameof(video.Trailer), input.TraileFile.Extension);
                var uploadFilePath = await _storageService.Upload(fileName, input.TraileFile.FileStream, cancellationToken);
                video.UpdateTrailer(uploadFilePath);
            }
        }

        private async Task UploadVideo(UploadMediasInput input, Domain.Entity.Video video, CancellationToken cancellationToken)
        {
            if (input.VideoFile is not null)
            {
                var fileName = StorageFileName.Create(input.Id, nameof(video.Media), input.VideoFile.Extension);
                var uploadFilePath = await _storageService.Upload(fileName, input.VideoFile.FileStream, cancellationToken);
                video.UpdateMedia(uploadFilePath);
            }
        }
    }
}
