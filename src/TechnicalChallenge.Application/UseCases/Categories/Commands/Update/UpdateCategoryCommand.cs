using MediatR;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Categories.Commands.Update;

public class UpdateCategoryCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public CategoryPurpose Purpose { get; set; }
    public Guid UserId { get; set; }
}
