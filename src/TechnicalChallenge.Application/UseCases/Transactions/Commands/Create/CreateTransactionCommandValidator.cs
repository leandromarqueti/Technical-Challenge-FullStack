using FluentValidation;
using TechnicalChallenge.Domain.Enums;

namespace TechnicalChallenge.Application.UseCases.Transactions.Commands.Create;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(400).WithMessage("A descrição deve ter no máximo 400 caracteres.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("A data da transação é obrigatória.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("O tipo da transação deve ser Receita (0) ou Despesa (1).");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("A categoria é obrigatória.");

        RuleFor(x => x.PersonId)
            .NotEmpty().WithMessage("A pessoa é obrigatória.");
    }
}
