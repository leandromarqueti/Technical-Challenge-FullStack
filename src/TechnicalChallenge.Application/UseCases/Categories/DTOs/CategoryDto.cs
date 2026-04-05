using TechnicalChallenge.Domain.Enums;

namespace TechnicalChallenge.Application.UseCases.Categories.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public CategoryPurpose Purpose { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
