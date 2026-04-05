using MediatR;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Categories.Commands.Update;

public class UpdateCategoryCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
