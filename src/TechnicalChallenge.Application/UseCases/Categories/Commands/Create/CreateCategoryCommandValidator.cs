using FluentValidation;

namespace TechnicalChallenge.Application.UseCases.Categories.Commands.Create;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(200).WithMessage("O nome da categoria deve ter no máximo 200 caracteres.");
    }
}
