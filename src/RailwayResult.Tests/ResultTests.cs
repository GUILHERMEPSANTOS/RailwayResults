using RailwayResults.Abstractions;
using RailwayResults.Errors;
using RailwayResults.Results;
using Xunit;

namespace RailwayResults.Tests.Results;

public class ResultTests
{
    [Fact]
    public void Success_DeveRetornarResultadoDeSucesso()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(ErrorType.None, result.Error.Type);
    }

    [Fact]
    public void Failure_DeveRetornarResultadoDeFalha()
    {
        var error = Error.Validation(
            "Validation.Required",
            "Campo obrigatório");

        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Construtor_DeveLancarExcecao_QuandoSucessoPossuirErro()
    {
        var ctor = typeof(Result)
            .GetConstructors(
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
            .Single();

        var exception = Assert.ThrowsAny<Exception>(() =>
            ctor.Invoke(new object[]
            {
                true,
                Error.Validation("CODE", "Mensagem")
            }));

        Assert.NotNull(exception);
    }
}