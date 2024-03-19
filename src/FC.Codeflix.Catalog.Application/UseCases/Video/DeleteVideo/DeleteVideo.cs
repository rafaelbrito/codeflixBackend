using FC.Codeflix.Catalog.Application.Inferfaces;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo
{
    public class DeleteVideo : IDeleteVideo
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public DeleteVideo(IVideoRepository videoRepository, IUnitOfWork unitOfWork, IStorageService storageService)
        {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }

        public async Task Handle(DeleteVideoInput input, CancellationToken cancellationToken)
        {
            var video = await _videoRepository.Get(input.VideoId, cancellationToken);
            await _videoRepository.Delete(video, cancellationToken);

            await ClearStorage(video, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);
        }

        private async Task ClearStorage(Domain.Entity.Video video, CancellationToken cancellationToken)
        {
            if (video.Trailer is not null)
                await _storageService.Delete(video.Trailer.FilePath, cancellationToken);

            if (video.Media is not null)
                await _storageService.Delete(video.Media.FilePath, cancellationToken);
        }
    }
}
