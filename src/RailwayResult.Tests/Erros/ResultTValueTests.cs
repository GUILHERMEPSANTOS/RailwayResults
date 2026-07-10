using RailwayResults.Abstractions;
using RailwayResults.Errors;
using RailwayResults.Results;

namespace RailwayResults.Tests.Results;

public class ResultTValueTests
{
    [Fact]
    public void Success_DeveRetornarValor()
    {
        var result = Result<string>.Success("Teste");

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal("Teste", result.Value);
        Assert.Equal(ErrorType.None, result.Error.Type);
    }

    [Fact]
    public void Failure_DeveRetornarErro()
    {
        var error = Error.NotFound(
            "User.NotFound",
            "Usuário não encontrado");

        var result = Result<string>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Create_DeveRetornarSuccess_QuandoValorNaoForNulo()
    {
        var result = Result<string>.Create("Valor");

        Assert.True(result.IsSuccess);
        Assert.Equal("Valor", result.Value);
    }

    [Fact]
    public void Create_DeveRetornarFailure_QuandoValorForNulo()
    {
        var result = Result<string>.Create(null);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(CommonErrors.NotFound.Code, result.Error.Code);
    }

    [Fact]
    public void Implicit_DeveConverterResultParaValor()
    {
        Result<string> result = Result<string>.Success("Teste");

        string value = result;

        Assert.Equal("Teste", value);
    }

    [Fact]
    public void Implicit_DeveConverterValorParaResult()
    {
        Result<string> result = "Teste";

        Assert.True(result.IsSuccess);
        Assert.Equal("Teste", result.Value);
    }

    [Fact]
    public void Implicit_DeveConverterNullParaFailure()
    {
        string value = null;

        Result<string> result = value;

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(CommonErrors.NotFound.Code, result.Error.Code);
    }

    [Fact]
    public void Implicit_DeveRetornarDefaultQuandoFailureForConvertidoParaValor()
    {
        Result<string> result =
            Result<string>.Failure(CommonErrors.NotFound);

        string value = result;

        Assert.Null(value);
    }
}