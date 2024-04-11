using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Validator;
using FC.Codeflix.Catalog.Domain.ValueObject;

namespace FC.Codeflix.Catalog.Domain.Entity
{
    public class Video : AggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int YearLauched { get; private set; }
        public bool Opened { get; private set; }
        public bool Published { get; private set; }
        public int Duration { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Rating Rating { get; private set; }
        public Image? Thumb { get; private set; }
        public Image? ThumbHalf { get; private set; }
        public Image? Banner { get; private set; }
        public Media? Media { get; private set; }
        public Media? Trailer { get; private set; }

        private List<Guid> _categories;
        public IReadOnlyList<Guid> Categories => _categories.AsReadOnly();
        private List<Guid> _genres;
        public IReadOnlyList<Guid> Genres => _genres.AsReadOnly();
        private List<Guid> _castMembers;
        public IReadOnlyList<Guid> CastMembers => _castMembers.AsReadOnly();

        public Video(
            string title,
            string description,
            int yearLauched,
            bool opened,
            bool published,
            int duration,
            Rating rating
            )

        {
            Title = title;
            Description = description;
            YearLauched = yearLauched;
            Opened = opened;
            Published = published;
            Duration = duration;
            Rating = rating;
            CreatedAt = DateTime.Now;
            _categories = new();
            _genres = new();
            _castMembers = new();
        }

        public void Validate(ValidationHandler handler)
            => new VideoValidator(this, handler).Validate();

        public void Update(
             string title,
             string description,
             int yearLauched,
             bool opened,
             bool published,
             int duration,
             Rating rating
            )
        {
            Title = title;
            Description = description;
            YearLauched = yearLauched;
            Opened = opened;
            Published = published;
            Duration = duration;
            Rating = rating;
        }

        public void UpdateThumb(string path)
            => Thumb = new Image(path);

        public void UpdateThumbHalf(string path)
           => ThumbHalf = new Image(path);

        public void UpdateBanner(string path)
           => Banner = new Image(path);

        public void UpdateMedia(string path)
        {
            Media = new Media(path);
            RaiseEvent(new VideoUploadedEvent(Id, path));
        }

        public void UpdateTrailer(string path)
            => Trailer = new Media(path);

        public void UpdateAsSentToEncode()
        {
            if (Media is null)
                throw new EntityValidationException("There is no media");
            Media.UpdateAsSentToEncode();
        }

        public void UpdateAsEncoded(string path)
        {
            if (Media is null)
                throw new EntityValidationException("There is no media");
            Media!.UpdateAsEncoded(path);
        }

        public void AddCategory(Guid id)
            => _categories.Add(id);

        public void RemoveCategory(Guid id)
            => _categories.Remove(id);

        public void RemoveAllCategory()
            => _categories.Clear();

        public void AddGenre(Guid id)
            => _genres.Add(id);

        public void RemoveGenre(Guid id)
            => _genres.Remove(id);

        public void RemoveAllGenres()
            => _genres.Clear();

        public void AddCastMember(Guid id)
            => _castMembers.Add(id);

        public void RemoveCastMember(Guid id)
            => _castMembers.Remove(id);

        public void RemoveAllCastMember()
            => _castMembers.Clear();
    }
}
