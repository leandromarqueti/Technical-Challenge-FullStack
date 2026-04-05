using FluentAssertions;
using Xunit;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Domain.Tests.Entities;

public class CategoryTests
{
    [Fact]
    public void Constructor_WithDescriptionExceeding400Characters_ShouldThrowDomainException()
    {
        //Arrange
        var longDescription = new string('C', 401);

        //Act
        Action action = () => new Category(longDescription);

        //Assert
        action.Should().Throw<DomainException>()
              .WithMessage("A descrição da categoria deve ter no máximo 400 caracteres.");
    }
}
