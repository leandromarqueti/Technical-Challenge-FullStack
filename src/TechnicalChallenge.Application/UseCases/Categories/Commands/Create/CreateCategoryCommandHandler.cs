using MediatR;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Interfaces;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Categories.Commands.Create;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        //Verifica se já existe categoria com o mesmo nome
        var existing = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing is not null)
        {
            return Result<Guid>.Failure("Já existe uma categoria com este nome.");
        }

        var category = new Category(request.Name);

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(category.Id, "Categoria criada com sucesso.");
    }
}
