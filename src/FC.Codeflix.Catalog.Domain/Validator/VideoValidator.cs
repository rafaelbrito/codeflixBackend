using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Validator
{
    public class VideoValidator : Validation.Validator
    {
        private readonly Video _video;
        private const int TITLE_MAX_LENGTH = 255;
        private const int DESCRIPTION_MAX_LENGTH = 4000;


        public VideoValidator(Video video, ValidationHandler handler) : base(handler)
          => _video = video;

        public override void Validate()
        {
            if (String.IsNullOrWhiteSpace(_video.Title))
                _handler.HandleError($"'{nameof(_video.Title)}' is required");

            if (_video.Title.Length > TITLE_MAX_LENGTH)
                _handler.HandleError($"'{nameof(_video.Title)}' should be less or equal {TITLE_MAX_LENGTH} characters long");

            if (String.IsNullOrWhiteSpace(_video.Description))
                _handler.HandleError($"'{nameof(_video.Description)}' is required");

            if (_video.Description.Length > DESCRIPTION_MAX_LENGTH)
                _handler.HandleError($"'{nameof(_video.Description)}' should be less or equal {DESCRIPTION_MAX_LENGTH} characters long");
        }
    }
}
