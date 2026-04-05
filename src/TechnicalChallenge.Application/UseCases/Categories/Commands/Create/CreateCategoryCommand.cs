using MediatR;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Categories.Commands.Create;

public class CreateCategoryCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; } = string.Empty;
}
