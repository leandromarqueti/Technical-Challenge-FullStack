using FluentAssertions;
using Moq;
using TechnicalChallenge.Application.UseCases.Transactions.Commands.Create;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Domain.Interfaces;
using Xunit;

namespace TechnicalChallenge.Application.Tests.UseCases.Transactions.Commands;

public class CreateTransactionCommandHandlerTests
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateTransactionCommandHandler _handler;

    public CreateTransactionCommandHandlerTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _personRepositoryMock = new Mock<IPersonRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CreateTransactionCommandHandler(
            _transactionRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _personRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateTransaction()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateTransactionCommand
        {
            Description = "Almoço",
            Amount = 50.0m,
            Date = DateTime.UtcNow,
            Type = TransactionType.Expense,
            CategoryId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            UserId = userId
        };

        var category = new Category("Alimentação", userId, CategoryPurpose.Expense);
        var person = new Person("João Silva", DateTime.UtcNow.AddYears(-20), "11144477735", userId);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(command.CategoryId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _personRepositoryMock.Setup(r => r.GetByIdAsync(command.PersonId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _transactionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MinorTryingToRegisterRevenue_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateTransactionCommand
        {
            Type = TransactionType.Revenue,
            CategoryId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            UserId = userId
        };

        var category = new Category("Salário", userId, CategoryPurpose.Revenue);
        var person = new Person("Menor de Idade", DateTime.UtcNow.AddYears(-10), "11144477735", userId);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(command.CategoryId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _personRepositoryMock.Setup(r => r.GetByIdAsync(command.PersonId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Pessoas menores de 18 anos só podem ter transações do tipo 'Despesa'");
        _transactionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_IncompatibleCategory_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateTransactionCommand
        {
            Type = TransactionType.Revenue,
            CategoryId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            UserId = userId
        };

        var category = new Category("Lazer", userId, CategoryPurpose.Expense); // Categoria só de despesa
        var person = new Person("Adulto", DateTime.UtcNow.AddYears(-30), "11144477735", userId);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(command.CategoryId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _personRepositoryMock.Setup(r => r.GetByIdAsync(command.PersonId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("A categoria selecionada não é compatível com este tipo de transação");
        _transactionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
