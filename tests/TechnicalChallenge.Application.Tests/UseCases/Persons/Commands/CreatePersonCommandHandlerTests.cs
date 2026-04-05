using FluentAssertions;
using Moq;
using TechnicalChallenge.Application.UseCases.Persons.Commands.Create;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Interfaces;
using Xunit;

namespace TechnicalChallenge.Application.Tests.UseCases.Persons.Commands;

public class CreatePersonCommandHandlerTests
{
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreatePersonCommandHandler _handler;

    public CreatePersonCommandHandlerTests()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreatePersonCommandHandler(_personRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreatePersonAndReturnSuccess()
    {
        //Arrange
        var command = new CreatePersonCommand
        {
            Name = "John Doe",
            BirthDate = new DateTime(1990, 1, 1),
            Document = "11144477735", //CPF Válido
            UserId = Guid.NewGuid()
        };

        _personRepositoryMock.Setup(r => r.ExistsByDocumentAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _personRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        //Act
        var result = await _handler.Handle(command, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty(); //Guid gerado validamente
        result.Message.Should().Be("Pessoa cadastrada com sucesso.");

        //Verifica se o repositório e o UnitOfWork foram chamados corretamente
        _personRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingDocument_ShouldReturnFailureResult()
    {
        //Arrange
        var command = new CreatePersonCommand
        {
            Name = "Jane Doe",
            BirthDate = new DateTime(1995, 5, 5),
            Document = "11144477735",
            UserId = Guid.NewGuid()
        };

        //Simula que o documento já existe no banco para este usuário
        _personRepositoryMock.Setup(r => r.ExistsByDocumentAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        //Act
        var result = await _handler.Handle(command, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Já existe uma pessoa cadastrada com este documento");

        //Verifica que NÃO chamou o AddAsync nem o SaveChanges
        _personRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
