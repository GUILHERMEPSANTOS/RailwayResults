using RailwayResults.Abstractions;
using RailwayResults.Errors;
using RailwayResults.Results;

namespace RailwayResults.Tests.Results;

public class ValidationResultTTests
{
    [Fact]
    public void Success_DeveRetornarResultadoComSucesso()
    {
        // Arrange
        const string valor = "Guilherme";

        // Act
        var result = ValidationResultT<string>.Success(valor);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(valor, result.Value);
        Assert.Empty(result.Errors);
        Assert.Equal(ErrorType.None, result.Error.Type);
    }

    [Fact]
    public void Failure_DeveRetornarResultadoComFalha()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation(
                "name.required",
                "Nome é obrigatório")
        };

        // Act
        var result = ValidationResultT<string>.Failure(errors);

        // Assert
        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);

        Assert.Single(result.Errors);

        Assert.Equal(
            CommonErrors.MultipleErrors,
            result.Error);
    }

    [Fact]
    public void Create_ComValorValido_DeveRetornarSuccess()
    {
        // Arrange
        const string valor = "Guilherme";

        // Act
        var result = ValidationResultT<string>.Create(valor);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(valor, result.Value);
    }

    [Fact]
    public void Create_ComValorNulo_DeveRetornarFailure()
    {
        // Act
        var result = ValidationResultT<string>.Create(null);

        // Assert
        Assert.True(result.IsFailure);

        Assert.Contains(
            result.Errors,
            x => x == CommonErrors.NotFound);
    }

    [Fact]
    public void OperadorImplicito_DeTValueParaValidationResultT_DeveConverter()
    {
        // Arrange
        const string valor = "Guilherme";

        // Act
        ValidationResultT<string> result = valor;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(valor, result.Value);
    }

    [Fact]
    public void OperadorImplicito_DeValidationResultTParaTValue_DeveRetornarValor()
    {
        // Arrange
        var result =
            ValidationResultT<string>.Success("Guilherme");

        // Act
        string valor = result;

        // Assert
        Assert.Equal("Guilherme", valor);
    }

    [Fact]
    public void Failure_ComMultiplosErros_DeveManterTodosOsErros()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation(
                "name.required",
                "Nome obrigatório"),

            Error.Validation(
                "email.required",
                "Email obrigatório")
        };

        // Act
        var result =
            ValidationResultT<string>.Failure(errors);

        // Assert
        Assert.Equal(2, result.Errors.Count());

        Assert.Equal(
            CommonErrors.MultipleErrors,
            result.Error);
    }

    [Fact]
    public void Success_DeveRetornarErrorNone()
    {
        // Act
        var result =
            ValidationResultT<string>.Success("Teste");

        // Assert
        Assert.Equal(
            ErrorType.None,
            result.Error.Type);
    }

    [Fact]
    public void Failure_DeveRetornarMultipleErrors()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation(
                "error.code",
                "Erro")
        };

        // Act
        var result =
            ValidationResultT<string>.Failure(errors);

        // Assert
        Assert.Equal(
            CommonErrors.MultipleErrors,
            result.Error);
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoSucessoPossuirErros()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation(
                "error.code",
                "Erro")
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new FakeValidationResult<string>(
                true,
                "valor",
                errors));
    }
}

file sealed class FakeValidationResult<TValue>
    : ValidationResultT<TValue>
{
    public FakeValidationResult(
        bool isSuccess,
        TValue value,
        IEnumerable<Error> errors)
        : base(isSuccess, value, errors)
    {
    }
}