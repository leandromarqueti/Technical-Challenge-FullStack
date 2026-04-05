using MediatR;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Categories.Commands.Delete;

public class DeleteCategoryCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }

    public DeleteCategoryCommand(Guid id)
    {
        Id = id;
    }
}
