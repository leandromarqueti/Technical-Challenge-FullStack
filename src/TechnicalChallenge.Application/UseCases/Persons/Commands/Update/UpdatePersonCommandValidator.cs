using FluentValidation;
using TechnicalChallenge.Domain.Validators;

namespace TechnicalChallenge.Application.UseCases.Persons.Commands.Update;

public class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
{
    public UpdatePersonCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da pessoa é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .LessThan(DateTime.UtcNow).WithMessage("A data de nascimento não pode ser no futuro.");

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("O documento (CPF/CNPJ) é obrigatório.")
            .Must(doc => DocumentValidator.IsValid(doc))
                .WithMessage("O documento informado (CPF/CNPJ) é inválido.");
    }
}
