using MediatR;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Categories.Commands.Create;

public class CreateCategoryCommand : IRequest<Result<Guid>>
{
    public string Description { get; set; } = string.Empty;
    public CategoryPurpose Purpose { get; set; }
    public Guid UserId { get; set; }
}
