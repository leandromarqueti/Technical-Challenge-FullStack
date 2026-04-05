using MediatR;
using TechnicalChallenge.Application.Common.Interfaces;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Shared.Results;

namespace TechnicalChallenge.Application.UseCases.Transactions.Commands.Update;

public class UpdateTransactionCommand : IRequest<Result<bool>>, IUserOwnedRequest
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public Guid CategoryId { get; set; }
    public Guid PersonId { get; set; }
    public Guid UserId { get; set; }
}
