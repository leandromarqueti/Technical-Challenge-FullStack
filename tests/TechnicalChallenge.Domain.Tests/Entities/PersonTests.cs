using FluentAssertions;
using Xunit;
using TechnicalChallenge.Domain.Entities;
using TechnicalChallenge.Shared.Exceptions;

namespace TechnicalChallenge.Domain.Tests.Entities;

public class PersonTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePerson()
    {
        //Arrange
        var name = "Leandro Marqueti";
        var birthDate = new DateTime(1990, 1, 1);
        var document = "123.456.789-00"; //Assuming valid in bypass or we mock it, wait, DocumentValidator is static in this project and expects math valid. I will use a mathematically valid CPF: "52998224725"

        //Act
        var person = new Person(name, birthDate, "52998224725");

        //Assert
        person.Should().NotBeNull();
        person.Name.Should().Be(name);
        person.BirthDate.Should().Be(birthDate);
        person.Document.Should().Be("52998224725"); //Sanitized
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowDomainException()
    {
        //Act
        Action action = () => new Person("", new DateTime(1990, 1, 1), "52998224725");

        //Assert
        action.Should().Throw<DomainException>()
              .WithMessage(ResourceErrorMessages.NAME_EMPTY);
    }

    [Fact]
    public void Constructor_WithNameExceeding200Characters_ShouldThrowDomainException()
    {
        //Arrange
        var longName = new string('A', 201);

        //Act
        Action action = () => new Person(longName, new DateTime(1990, 1, 1), "52998224725");

        //Assert
        action.Should().Throw<DomainException>()
              .WithMessage("O nome deve ter no máximo 200 caracteres.");
    }
}
