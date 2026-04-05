using MediatR;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Commands.Create;

public class CreateTransactionCommand : IRequest<Result<Guid>>
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public Guid CategoryId { get; set; }
    public Guid PersonId { get; set; }
}
