using MediatR;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Exceptions;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Categories.Commands.Update;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException("Categoria", request.Id);
        }

        //Verifica se a nova descrição já está em uso por outra categoria
        var existing = await _categoryRepository.GetByDescriptionAsync(request.Description, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            return Result<bool>.Failure("Já existe outra categoria com esta descrição.");
        }

        category.Update(request.Description, request.Purpose);

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Categoria atualizada com sucesso.");
    }
}
