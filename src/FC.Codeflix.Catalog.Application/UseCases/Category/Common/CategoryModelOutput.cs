﻿using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.Common
{
    public class CategoryModelOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public CategoryModelOutput(
            Guid id,
            string name,
            string description,
            bool isActive,
            DateTime createAt)
        {
            Id = id;
            Name = name;
            Description = description;
            IsActive = isActive;
            CreatedAt = createAt;
        }

        public static CategoryModelOutput FromCategory(DomainEntity.Category category)
        => new(
                            category.Id,
                            category.Name,
                            category.Description,
                            category.IsActive,
                            category.CreatedAt
            );
    }
}
