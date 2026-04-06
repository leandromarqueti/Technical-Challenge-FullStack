using MediatR;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Categories.Commands.Create;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository, 
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existing = await categoryRepository.GetByDescriptionAsync(request.Description, request.UserId, cancellationToken);
        if (existing is not null)
        {
            return Result<Guid>.Failure("Já existe uma categoria com esta descrição.");
        }

        var category = new Category(request.Description, request.UserId, request.Purpose);

        await categoryRepository.AddAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(category.Id, "Categoria criada com sucesso.");
    }
}
