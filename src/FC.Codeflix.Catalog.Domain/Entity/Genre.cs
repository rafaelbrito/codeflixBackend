using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Entity
{
    public class Genre : AggregateRoot
    {
        public string Name { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private List<Guid> _categories;
        public IReadOnlyList<Guid> Categories => _categories.AsReadOnly();

        public Genre(string name, bool isActive = true)
        {
            Name = name;
            IsActive = isActive;
            CreatedAt = DateTime.Now;
            _categories = new List<Guid>();
            Validate();
        }

        public void Activate()
        {
            IsActive = true;
            Validate();
        }

        public void Deactivate()
        {
            IsActive = false;
            Validate();
        }

        public void Update(string name)
        {
            Name = name;
            Validate();
        }

        private void Validate()
            => DomainValidation.NotNullOrEmpty(Name, nameof(Name));

        public void AddCategory(Guid id)
        {
            _categories.Add(id);
            Validate();
        }

        public void RemoveCategory(Guid id)
        {
            _categories.Remove(id);
            Validate();
        }

        public void RemoveAllCategory()
        {
            _categories.Clear();
            Validate();
        }
    }
}
