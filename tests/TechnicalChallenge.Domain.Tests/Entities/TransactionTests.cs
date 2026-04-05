using FluentAssertions;
using Xunit;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Domain.Tests.Entities;

public class TransactionTests
{
    [Fact]
    public void Transaction_ShouldHaveAllProperties()
    {
        //Verifica se os dados da transação estão corretos
        var description = "Almoço";
        var amount = 50.0m;
        var type = TransactionType.Expense;
        var date = DateTime.Now;
        var categoryId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var transaction = new Transaction(description, amount, date, type, categoryId, personId, userId);

        //Validação das propriedades
        Assert.Equal(description, transaction.Description);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(type, transaction.Type);
    }

    [Fact]
    public void Constructor_WithDescriptionExceeding400Characters_ShouldThrowDomainException()
    {
        //Arrange
        var longDescription = new string('B', 401);

        //Act
        Action action = () => new Transaction(
            description: longDescription,
            amount: 100.50m,
            date: DateTime.UtcNow,
            type: TransactionType.Expense,
            categoryId: Guid.NewGuid(),
            personId: Guid.NewGuid(),
            userId: Guid.NewGuid()
        );

        //Assert
        action.Should().Throw<DomainException>()
              .WithMessage("A descrição deve ter no máximo 400 caracteres.");
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowDomainException()
    {
        //Act
        Action action = () => new Transaction(
            description: "Compra no mercado",
            amount: -10m,
            date: DateTime.UtcNow,
            type: TransactionType.Expense,
            categoryId: Guid.NewGuid(),
            personId: Guid.NewGuid(),
            userId: Guid.NewGuid()
        );

        //Assert
        action.Should().Throw<DomainException>()
              .WithMessage("O valor da transação deve ser maior que zero.");
    }
}
