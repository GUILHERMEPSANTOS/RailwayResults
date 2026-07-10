using RailwayResults.Abstractions;
using Error = RailwayResults.Errors.Error;

namespace RailwayResult.Tests.Errors;

public class ErrorTests
{
    [Fact]
    public void Validation_DeveCriarErroDeValidacao()
    {
        var error = Error.Validation("VAL001", "Campo obrigatório");

        Assert.Equal("VAL001", error.Code);
        Assert.Equal("Campo obrigatório", error.Message);
        Assert.Equal(ErrorType.Validation, error.Type);
    }

    [Fact]
    public void NotFound_DeveCriarErroNotFound()
    {
        var error = Error.NotFound("NF001", "Registro não encontrado");

        Assert.Equal("NF001", error.Code);
        Assert.Equal("Registro não encontrado", error.Message);
        Assert.Equal(ErrorType.NotFound, error.Type);
    }

    [Fact]
    public void Conflict_DeveCriarErroConflict()
    {
        var error = Error.Conflict("CF001", "Conflito encontrado");

        Assert.Equal("CF001", error.Code);
        Assert.Equal("Conflito encontrado", error.Message);
        Assert.Equal(ErrorType.Conflict, error.Type);
    }

    [Fact]
    public void Unauthorized_DeveCriarErroUnauthorized()
    {
        var error = Error.Unauthorized("AUTH001", "Acesso negado");

        Assert.Equal("AUTH001", error.Code);
        Assert.Equal("Acesso negado", error.Message);
        Assert.Equal(ErrorType.Unauthorized, error.Type);
    }

    [Fact]
    public void None_DeveRetornarErroVazio()
    {
        var error = Error.None();

        Assert.Equal(string.Empty, error.Code);
        Assert.Equal(string.Empty, error.Message);
        Assert.Equal(ErrorType.None, error.Type);
    }

    [Theory]
    [InlineData("CODE", "")]
    [InlineData("", "MESSAGE")]
    [InlineData("CODE", "MESSAGE")]
    public void Construtor_DeveLancarArgumentException_QuandoTypeForNoneECodeOuMessagePossuiremValor(
        string code,
        string message)
    {
        var ctor = typeof(Error)
            .GetConstructors(System.Reflection.BindingFlags.NonPublic |
                             System.Reflection.BindingFlags.Instance)
            .Single();

        var exception = Assert.ThrowsAny<Exception>(() =>
            ctor.Invoke(new object[]
            {
                code,
                message,
                ErrorType.None
            }));

        Assert.NotNull(exception);
    }
}