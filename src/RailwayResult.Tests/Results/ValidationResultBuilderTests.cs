using RailwayResults.Abstractions;
using RailwayResults.Errors;
using RailwayResults.Results;

namespace RailwayResults.Tests.Results;

public class ValidationResultBuilderTests
{
    [Fact]
    public void Create_DeveRetornarNovaInstancia()
    {
        // Act
        var builder = ValidationResultBuilder.Create();

        // Assert
        Assert.NotNull(builder);
        Assert.IsType<ValidationResultBuilder>(builder);
    }

    [Fact]
    public void Build_SemChecks_DeveRetornarSuccess()
    {
        // Act
        var result = ValidationResultBuilder
            .Create()
            .Build();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void CheckNotNull_ComValorNulo_DeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation(
            "Responsavel.Required",
            "Responsável é obrigatório");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotNull<object>(null, error)
            .Build();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.Code == "Responsavel.Required");
    }

    [Fact]
    public void CheckNotNull_ComValorNaoNulo_NaoDeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation(
            "Responsavel.Required",
            "Responsável é obrigatório");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotNull("valor", error)
            .Build();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void CheckNotEmpty_String_ComValorNulo_DeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation(
            "Nome.Required",
            "Nome é obrigatório");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotEmpty((string)null!, error)
            .Build();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.Code == "Nome.Required");
    }

    [Fact]
    public void CheckNotEmpty_String_ComValorVazio_DeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation(
            "Nome.Required",
            "Nome é obrigatório");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotEmpty(string.Empty, error)
            .Build();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.Code == "Nome.Required");
    }

    [Fact]
    public void CheckNotEmpty_String_ComValorPreenchido_NaoDeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation(
            "Nome.Required",
            "Nome é obrigatório");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotEmpty("Empresa Teste", error)
            .Build();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void CheckNotEmpty_String_ComApenasEspacosEmBranco_NaoDeveAdicionarErro()
    {
        // Arrange: string.IsNullOrEmpty (usado na implementação atual) não considera
        // espaços em branco como vazio — diferente de string.IsNullOrWhiteSpace.
        // Este teste documenta esse comportamento atual do CheckNotEmpty(string, Error).
        var error = Error.Validation(
            "Nome.Required",
            "Nome é obrigatório");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotEmpty("   ", error)
            .Build();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void CheckNotEmpty_Bool_ComValorFalso_DeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation(
            "Nome.Invalido",
            "Nome deve ter entre 3 e 150 caracteres");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotEmpty(false, error)
            .Build();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.Code == "Nome.Invalido");
    }

    [Fact]
    public void CheckNotEmpty_Bool_ComValorVerdadeiro_NaoDeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation(
            "Nome.Invalido",
            "Nome deve ter entre 3 e 150 caracteres");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotEmpty(true, error)
            .Build();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Build_ComMultiplosChecksFalhando_DeveAcumularTodosOsErros()
    {
        // Arrange
        var erroNome = Error.Validation("Nome.Required", "Nome é obrigatório");
        var erroResponsavel = Error.Validation("Responsavel.Required", "Responsável é obrigatório");
        var erroSegmento = Error.Validation("Segmento.Required", "Segmento é obrigatório");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotEmpty(string.Empty, erroNome)
            .CheckNotNull<object>(null, erroResponsavel)
            .CheckNotNull<object>(null, erroSegmento)
            .Build();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(3, result.Errors.Count());
        Assert.Contains(result.Errors, e => e.Code == "Nome.Required");
        Assert.Contains(result.Errors, e => e.Code == "Responsavel.Required");
        Assert.Contains(result.Errors, e => e.Code == "Segmento.Required");
    }

    [Fact]
    public void Build_ComChecksMistosSucessoEFalha_DeveAdicionarApenasOsQueFalharam()
    {
        // Arrange
        var erroNome = Error.Validation("Nome.Required", "Nome é obrigatório");
        var erroResponsavel = Error.Validation("Responsavel.Required", "Responsável é obrigatório");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotEmpty("Empresa Válida", erroNome) // não deve falhar
            .CheckNotNull<object>(null, erroResponsavel) // deve falhar
            .Build();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.Code == "Responsavel.Required");
        Assert.DoesNotContain(result.Errors, e => e.Code == "Nome.Required");
    }

    [Fact]
    public void Build_ComTodosOsChecksValidos_DeveRetornarSuccess()
    {
        // Arrange
        var erroNome = Error.Validation("Nome.Required", "Nome é obrigatório");
        var erroResponsavel = Error.Validation("Responsavel.Required", "Responsável é obrigatório");
        var erroSegmento = Error.Validation("Segmento.Invalido", "Segmento inválido");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotEmpty("Empresa Válida", erroNome)
            .CheckNotNull("Responsavel", erroResponsavel)
            .CheckNotEmpty(true, erroSegmento)
            .Build();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
        Assert.Equal(ErrorType.None, result.Error.Type);
    }

    [Fact]
    public void Build_ComFalha_DeveRetornarMultipleErrorsComoErroPrincipal()
    {
        // Arrange
        var error = Error.Validation("Nome.Required", "Nome é obrigatório");

        // Act
        var result = ValidationResultBuilder
            .Create()
            .CheckNotEmpty(string.Empty, error)
            .Build();

        // Assert
        Assert.Equal(CommonErrors.MultipleErrors.Code, result.Error.Code);
        Assert.Equal(CommonErrors.MultipleErrors.Message, result.Error.Message);
    }

    [Fact]
    public void Metodos_DevemRetornarMesmaInstancia_ParaPermitirEncadeamentoFluente()
    {
        // Arrange
        var builder = ValidationResultBuilder.Create();
        var error = Error.Validation("Codigo", "Mensagem");

        // Act
        var retornoCheckNotNull = builder.CheckNotNull("valor", error);
        var retornoCheckNotEmptyString = builder.CheckNotEmpty("valor", error);
        var retornoCheckNotEmptyBool = builder.CheckNotEmpty(true, error);

        // Assert
        Assert.Same(builder, retornoCheckNotNull);
        Assert.Same(builder, retornoCheckNotEmptyString);
        Assert.Same(builder, retornoCheckNotEmptyBool);
    }

    [Fact]
    public void Build_ChamadoMaisDeUmaVez_DeveRetornarResultadosConsistentes()
    {
        // Arrange
        var error = Error.Validation("Nome.Required", "Nome é obrigatório");
        var builder = ValidationResultBuilder
            .Create()
            .CheckNotEmpty(string.Empty, error);

        // Act
        var primeiraChamada = builder.Build();
        var segundaChamada = builder.Build();

        // Assert
        Assert.False(primeiraChamada.IsSuccess);
        Assert.False(segundaChamada.IsSuccess);
        Assert.Equal(primeiraChamada.Errors.Count(), segundaChamada.Errors.Count());
    }
}