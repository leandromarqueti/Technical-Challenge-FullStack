using FluentValidation;

namespace TechnicalChallenge.Application.UseCases.Categories.Commands.Create;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição da categoria é obrigatória.")
            .MaximumLength(400).WithMessage("A descrição da categoria deve ter no máximo 400 caracteres.");

        RuleFor(x => x.Purpose)
            .IsInEnum().WithMessage("Finalidade da categoria inválida.");
    }
}
