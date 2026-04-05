using TechnicalChallenge.Domain.Enums;

namespace TechnicalChallenge.Application.UseCases.Transactions.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public string TypeDescription => Type == TransactionType.Revenue ? "Receita" : "Despesa";

    public Guid CategoryId { get; set; }
    public string CategoryDescription { get; set; } = string.Empty;

    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
