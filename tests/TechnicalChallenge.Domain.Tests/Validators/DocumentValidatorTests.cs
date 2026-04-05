using FluentAssertions;
using TechnicalChallenge.Domain.Validators;
using Xunit;

namespace TechnicalChallenge.Domain.Tests.Validators;

public class DocumentValidatorTests
{
    [Theory]
    [InlineData("11144477735")] //CPF válido 
    [InlineData("111.444.777-35")] //CPF válido com máscara
    public void IsValid_WithValidCpf_ShouldReturnTrue(string cpf)
    {
        //Act
        var result = DocumentValidator.IsValid(cpf);

        //Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("11111111111")] //Sequência repetida
    [InlineData("10433241032")] //Dígito inválido
    [InlineData("123456789")] //Tamanho inválido
    [InlineData("")]
    [InlineData(null)]
    public void IsValid_WithInvalidCpf_ShouldReturnFalse(string invalidCpf)
    {
        //Act
        var result = DocumentValidator.IsValid(invalidCpf);

        //Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("00000000000191")] //CNPJ válido 
    [InlineData("00.000.000/0001-91")] //CNPJ válido com máscara
    public void IsValid_WithValidCnpj_ShouldReturnTrue(string cnpj)
    {
        //Act
        var result = DocumentValidator.IsValid(cnpj);

        //Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("11111111111111")] //Sequência repetida
    [InlineData("71274533000108")] //Dígito inválido
    [InlineData("1234567890123")] //Tamanho inválido
    public void IsValid_WithInvalidCnpj_ShouldReturnFalse(string invalidCnpj)
    {
        //Act
        var result = DocumentValidator.IsValid(invalidCnpj);

        //Assert
        result.Should().BeFalse();
    }
}
