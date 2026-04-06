using FluentAssertions;
using Moq;
using TechnicalChallenge.Application.UseCases.Categories.Commands.Create;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Domain.Enums;
using TechnicalChallenge.Domain.Interfaces;
using Xunit;

namespace TechnicalChallenge.Application.Tests.UseCases.Categories.Commands;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateCategoryCommandHandler(_categoryRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateCategory()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateCategoryCommand
        {
            Description = "Educação",
            Purpose = CategoryPurpose.Expense,
            UserId = userId
        };

        _categoryRepositoryMock.Setup(r => r.GetByDescriptionAsync(command.Description, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateDescription_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateCategoryCommand
        {
            Description = "Educação",
            UserId = userId
        };

        var existingCategory = new Category("Educação", userId, CategoryPurpose.Both);

        _categoryRepositoryMock.Setup(r => r.GetByDescriptionAsync(command.Description, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Já existe uma categoria com esta descrição.");
        _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
