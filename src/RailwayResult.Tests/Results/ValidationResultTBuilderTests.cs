using RailwayResults.Abstractions;
using RailwayResults.Errors;
using RailwayResults.Results;
using Xunit;

namespace RailwayResults.Tests.Results;

public class ValidationResultTBuilderTests
{
    [Fact]
    public void Create_DeveRetornarNovaInstancia()
    {
        // Act
        var builder = ValidationResultBuilder<string>.Create();

        // Assert
        Assert.NotNull(builder);
        Assert.IsType<ValidationResultBuilder<string>>(builder);
    }

    [Fact]
    public void Build_ComValor_SemChecks_DeveRetornarSuccessComOValor()
    {
        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .Build("Guilherme");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal("Guilherme", result.Value);
        Assert.Empty(result.Errors);
        Assert.Equal(ErrorType.None, result.Error.Type);
    }

    [Fact]
    public void Build_ComFactory_SemChecks_DeveRetornarSuccessComValorDaFactory()
    {
        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .Build(() => "Guilherme");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Guilherme", result.Value);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Build_ComFactory_QuandoValidacaoFalha_NaoDeveExecutarAFactory()
    {
        // Arrange
        var error = Error.Validation("Nome.Required", "Nome é obrigatório");
        var factoryFoiExecutada = false;

        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotEmpty(string.Empty, error)
            .Build(() =>
            {
                factoryFoiExecutada = true;
                return "Guilherme";
            });

        // Assert
        Assert.True(result.IsFailure);
        Assert.False(factoryFoiExecutada);
    }

    [Fact]
    public void Build_ComValor_QuandoValidacaoFalha_DeveRetornarFailureComErros()
    {
        // Arrange
        var error = Error.Validation("Nome.Required", "Nome é obrigatório");

        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotEmpty(string.Empty, error)
            .Build("Guilherme");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.Code == "Nome.Required");
    }

    [Fact]
    public void CheckNotNull_ComValorNulo_DeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation("Responsavel.Required", "Responsável é obrigatório");

        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotNull<object>(null, error)
            .Build("valor");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.Code == "Responsavel.Required");
    }

    [Fact]
    public void CheckNotNull_ComValorNaoNulo_NaoDeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation("Responsavel.Required", "Responsável é obrigatório");

        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotNull("responsavel", error)
            .Build("valor");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void CheckNotEmpty_String_ComValorVazio_DeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation("Nome.Required", "Nome é obrigatório");

        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotEmpty(string.Empty, error)
            .Build("valor");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.Code == "Nome.Required");
    }

    [Fact]
    public void CheckNotEmpty_String_ComValorNulo_DeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation("Nome.Required", "Nome é obrigatório");

        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotEmpty((string)null!, error)
            .Build("valor");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.Code == "Nome.Required");
    }

    [Fact]
    public void CheckNotEmpty_String_ComValorPreenchido_NaoDeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation("Nome.Required", "Nome é obrigatório");

        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotEmpty("Empresa Teste", error)
            .Build("valor");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void CheckNotEmpty_Bool_ComValorFalso_DeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation("Nome.Invalido", "Nome deve ter entre 3 e 150 caracteres");

        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotEmpty(false, error)
            .Build("valor");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.Code == "Nome.Invalido");
    }

    [Fact]
    public void CheckNotEmpty_Bool_ComValorVerdadeiro_NaoDeveAdicionarErro()
    {
        // Arrange
        var error = Error.Validation("Nome.Invalido", "Nome deve ter entre 3 e 150 caracteres");

        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotEmpty(true, error)
            .Build("valor");

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
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotEmpty(string.Empty, erroNome)
            .CheckNotNull<object>(null, erroResponsavel)
            .CheckNotEmpty(false, erroSegmento)
            .Build("valor");

        // Assert
        Assert.True(result.IsFailure);
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
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotEmpty("Empresa Válida", erroNome) // não deve falhar
            .CheckNotNull<object>(null, erroResponsavel) // deve falhar
            .Build("valor");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.Code == "Responsavel.Required");
        Assert.DoesNotContain(result.Errors, e => e.Code == "Nome.Required");
    }

    [Fact]
    public void Build_ComFalha_DeveUsarMultipleErrorsComoErroPrincipal()
    {
        // Arrange
        var error = Error.Validation("Nome.Required", "Nome é obrigatório");

        // Act
        var result = ValidationResultBuilder<string>
            .Create()
            .CheckNotEmpty(string.Empty, error)
            .Build("valor");

        // Assert
        Assert.Equal(CommonErrors.MultipleErrors.Code, result.Error.Code);
        Assert.Equal(CommonErrors.MultipleErrors.Message, result.Error.Message);
    }

    [Fact]
    public void Metodos_DevemManterOTipoGenerico_AoLongoDoEncadeamentoFluente()
    {
        // Arrange
        var builder = ValidationResultBuilder<string>.Create();
        var error = Error.Validation("Codigo", "Mensagem");

        // Act
        var retornoCheckNotNull = builder.CheckNotNull("valor", error);
        var retornoCheckNotEmptyString = builder.CheckNotEmpty("valor", error);
        var retornoCheckNotEmptyBool = builder.CheckNotEmpty(true, error);

        // Assert: garante que o "new" hiding preservou o tipo genérico
        // (permitindo chamar Build(TValue)/Build(Func<TValue>) no final da cadeia)
        Assert.IsType<ValidationResultBuilder<string>>(retornoCheckNotNull);
        Assert.IsType<ValidationResultBuilder<string>>(retornoCheckNotEmptyString);
        Assert.IsType<ValidationResultBuilder<string>>(retornoCheckNotEmptyBool);
        Assert.Same(builder, retornoCheckNotNull);
        Assert.Same(builder, retornoCheckNotEmptyString);
        Assert.Same(builder, retornoCheckNotEmptyBool);
    }

    [Fact]
    public void Build_ComTiposComplexos_DeveConstruirOValorCorretamenteViaFactory()
    {
        // Arrange: simula a criação de uma entidade cujo construtor só
        // deveria ser chamado após a validação passar.
        var nome = "Empresa Teste";
        var erroNome = Error.Validation("Nome.Required", "Nome é obrigatório");

        // Act
        var result = ValidationResultBuilder<Pessoa>
            .Create()
            .CheckNotEmpty(nome, erroNome)
            .Build(() => new Pessoa(nome));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(nome, result.Value.Nome);
    }

    private sealed class Pessoa
    {
        public string Nome { get; }

        public Pessoa(string nome)
        {
            Nome = nome;
        }
    }
}