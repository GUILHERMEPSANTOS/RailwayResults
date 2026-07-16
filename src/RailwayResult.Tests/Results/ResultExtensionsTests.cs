using RailwayResults.Abstractions;
using RailwayResults.Errors;
using RailwayResults.Results;

namespace RailwayResult.Tests.Results
{
    public class ResultExtensionsTests
    {
        [Fact]
        public void Failure_DeveRetornarResultadoDeFalha()
        {
            // Arrange
            var error = Error.Validation("Validation.Required", "Campo obrigatório");
            var result = Result<string>.Failure(error);

            // Act
            var result2 = result.Bind(valor => Result<int>.Success(1));

            // Assert
            Assert.False(result2.IsSuccess);
            Assert.True(result2.IsFailure);
            Assert.Same(error, result2.Error);
        }

        [Fact]
        public void Success_DeveRetornarResultadoDeSucesso()
        {
            // Arrange
            var result = Result<string>.Success("Valor");

            // Act
            var result2 = result.Bind(valor => Result<int>.Success(1));

            // Assert
            Assert.True(result2.IsSuccess);
            Assert.False(result2.IsFailure);
            Assert.Equal(1, result2.Value);
        }

        [Fact]
        public void Bind_QuandoFalha_NaoDeveExecutarAFuncaoNext()
        {
            // Arrange
            var error = Error.Validation("Validation.Required", "Campo obrigatório");
            var result = Result<string>.Failure(error);
            var proximaFuncaoFoiExecutada = false;

            // Act
            var result2 = result.Bind(valor =>
            {
                proximaFuncaoFoiExecutada = true;
                return Result<int>.Success(1);
            });

            // Assert
            Assert.True(result2.IsFailure);
            Assert.False(proximaFuncaoFoiExecutada);
        }

        [Fact]
        public void Bind_QuandoSucesso_DeveExecutarAFuncaoNext()
        {
            // Arrange
            var result = Result<string>.Success("Valor");
            var proximaFuncaoFoiExecutada = false;

            // Act
            var result2 = result.Bind(valor =>
            {
                proximaFuncaoFoiExecutada = true;
                return Result<int>.Success(1);
            });

            // Assert
            Assert.True(proximaFuncaoFoiExecutada);
            Assert.True(result2.IsSuccess);
        }

        [Fact]
        public void Bind_QuandoSucesso_DeveRepassarOValorOriginalParaAFuncaoNext()
        {
            // Arrange
            var result = Result<string>.Success("Guilherme");
            string? valorRecebido = null;

            // Act
            result.Bind(valor =>
            {
                valorRecebido = valor;
                return Result<int>.Success(valor.Length);
            });

            // Assert
            Assert.Equal("Guilherme", valorRecebido);
        }

        [Fact]
        public void Bind_QuandoSucesso_ComNextRetornandoFailure_DeveRetornarFailure()
        {
            // Arrange
            var result = Result<string>.Success("Valor");
            var erroDoProximoPasso = Error.Validation("Next.Invalido", "Próximo passo inválido");

            // Act
            var result2 = result.Bind(valor => Result<int>.Failure(erroDoProximoPasso));

            // Assert
            Assert.False(result2.IsSuccess);
            Assert.True(result2.IsFailure);
            Assert.Same(erroDoProximoPasso, result2.Error);
        }

        [Fact]
        public void Bind_DeveTransformarOTipoDoValor()
        {
            // Arrange
            var result = Result<string>.Success("Guilherme");

            // Act
            var result2 = result.Bind(valor => Result<int>.Success(valor.Length));

            // Assert
            Assert.True(result2.IsSuccess);
            Assert.Equal(9, result2.Value);
            Assert.IsType<int>(result2.Value);
        }

        [Fact]
        public void Bind_EncadeadoComTodosOsPassosDeSucesso_DeveRetornarSucessoFinal()
        {
            // Arrange
            var result = Result<string>.Success("10");

            // Act
            var resultadoFinal = result
                .Bind(valor => Result<int>.Success(int.Parse(valor)))
                .Bind(numero => Result<int>.Success(numero * 2))
                .Bind(numero => Result<string>.Success($"Total: {numero}"));

            // Assert
            Assert.True(resultadoFinal.IsSuccess);
            Assert.Equal("Total: 20", resultadoFinal.Value);
        }

        [Fact]
        public void Bind_EncadeadoQuandoPrimeiroPassoFalha_DeveParaNoPrimeiroPasso()
        {
            // Arrange
            var error = Error.Validation("Parse.Invalido", "Não foi possível converter o valor");
            var result = Result<string>.Failure(error);
            var segundoPassoFoiExecutado = false;
            var terceiroPassoFoiExecutado = false;

            // Act
            var resultadoFinal = result
                .Bind(valor => Result<int>.Success(int.Parse(valor)))
                .Bind(numero =>
                {
                    segundoPassoFoiExecutado = true;
                    return Result<int>.Success(numero * 2);
                })
                .Bind(numero =>
                {
                    terceiroPassoFoiExecutado = true;
                    return Result<string>.Success($"Total: {numero}");
                });

            // Assert
            Assert.True(resultadoFinal.IsFailure);
            Assert.Same(error, resultadoFinal.Error);
            Assert.False(segundoPassoFoiExecutado);
            Assert.False(terceiroPassoFoiExecutado);
        }

        [Fact]
        public void Bind_EncadeadoQuandoPassoIntermediarioFalha_DeveRetornarErroDoPassoQueFalhouEPararOsSeguintes()
        {
            // Arrange
            var erroIntermediario = Error.Validation("Numero.Invalido", "Número inválido");
            var result = Result<string>.Success("10");
            var terceiroPassoFoiExecutado = false;

            // Act
            var resultadoFinal = result
                .Bind(valor => Result<int>.Success(int.Parse(valor)))
                .Bind(numero => Result<int>.Failure(erroIntermediario))
                .Bind(numero =>
                {
                    terceiroPassoFoiExecutado = true;
                    return Result<string>.Success($"Total: {numero}");
                });

            // Assert
            Assert.True(resultadoFinal.IsFailure);
            Assert.Same(erroIntermediario, resultadoFinal.Error);
            Assert.False(terceiroPassoFoiExecutado);
        }

        [Fact]
        public void Bind_QuandoSucesso_DeveManterErrorNoneNoResultadoFinal()
        {
            // Arrange
            var result = Result<string>.Success("Valor");

            // Act
            var result2 = result.Bind(valor => Result<int>.Success(1));

            // Assert
            Assert.Equal(ErrorType.None, result2.Error.Type);
        }

        [Fact]
        public void Bind_ComTipoDeSaidaComplexo_DeveConstruirOValorCorretamente()
        {
            // Arrange
            var result = Result<string>.Success("Guilherme");

            // Act
            var result2 = result.Bind(nome => Result<Pessoa>.Success(new Pessoa(nome)));

            // Assert
            Assert.True(result2.IsSuccess);
            Assert.Equal("Guilherme", result2.Value.Nome);
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
}