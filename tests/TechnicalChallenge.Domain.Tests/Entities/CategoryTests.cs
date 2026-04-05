using FluentAssertions;
using Xunit;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Domain.Tests.Entities;

public class CategoryTests
{
    [Fact]
    public void Constructor_WithNameExceeding200Characters_ShouldThrowDomainException()
    {
        //Arrange
        var longName = new string('C', 201);

        //Act
        Action action = () => new Category(longName);

        //Assert
        action.Should().Throw<DomainException>()
              .WithMessage("O nome da categoria deve ter no máximo 200 caracteres.");
    }
}
