using RailwayResults.Abstractions;
using RailwayResults.Errors;
using RailwayResults.Results;
using Xunit;

namespace RailwayResults.Tests.Results;

public class ValidationResultTests
{
    [Fact]
    public void Success_DeveRetornarResultadoDeSucesso()
    {
        // Act
        var result = ValidationResult.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
        Assert.Equal(ErrorType.None, result.Error.Type);
    }

    [Fact]
    public void Failure_DeveRetornarResultadoDeFalha()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation("Name.Required", "Nome é obrigatório")
        };

        // Act
        var result = ValidationResult.Failure(errors);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Equal(CommonErrors.MultipleErrors.Code, result.Error.Code);
    }

    [Fact]
    public void Failure_DeveArmazenarTodosOsErros()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation("Name.Required", "Nome é obrigatório"),
            Error.Validation("Email.Required", "Email é obrigatório")
        };

        // Act
        var result = ValidationResult.Failure(errors);

        // Assert
        Assert.Equal(2, result.Errors.Count());
        Assert.Contains(result.Errors, e => e.Code == "Name.Required");
        Assert.Contains(result.Errors, e => e.Code == "Email.Required");
    }

    [Fact]
    public void Failure_ComColecaoVazia_DeveRetornarFalha()
    {
        // Act
        var result = ValidationResult.Failure([]);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Success_DevePossuirErrorNone()
    {
        // Act
        var result = ValidationResult.Success();

        // Assert
        Assert.Equal(ErrorType.None, result.Error.Type);
        Assert.Equal(string.Empty, result.Error.Code);
        Assert.Equal(string.Empty, result.Error.Message);
    }

    [Fact]
    public void Failure_ComMultiplosErros_DeveUtilizarMultipleErrors()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation("Name.Required", "Nome é obrigatório"),
            Error.Validation("Email.Required", "Email é obrigatório")
        };

        // Act
        var result = ValidationResult.Failure(errors);

        // Assert
        Assert.Equal(CommonErrors.MultipleErrors.Code, result.Error.Code);
        Assert.Equal(CommonErrors.MultipleErrors.Message, result.Error.Message);
    }
}