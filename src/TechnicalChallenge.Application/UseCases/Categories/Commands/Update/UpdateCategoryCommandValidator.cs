using FluentValidation;

namespace TechnicalChallenge.Application.UseCases.Categories.Commands.Update;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da categoria é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(200).WithMessage("O nome da categoria deve ter no máximo 200 caracteres.");
    }
}
