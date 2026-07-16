using RailwayResults.Results;

namespace RailwayResult.Tests.Results
{
    public static partial class ResultExtensions
    {
        extension<TValue>(Result<TValue> result)
        {
            /// <summary>
            /// Continua o fluxo de execução no trilho de sucesso,
            /// executando a operação informada somente quando o resultado atual
            /// representa uma operação bem-sucedida.
            /// </summary>
            /// <typeparam name="TNext">
            /// Tipo do valor produzido pela próxima etapa do pipeline.
            /// </typeparam>
            /// <param name="next">
            /// Operação que recebe o valor atual e retorna um novo resultado.
            /// </param>
            /// <returns>
            /// Um novo resultado contendo o valor produzido pela próxima operação
            /// ou a falha propagada da etapa anterior.
            /// </returns>
            public Result<TOut> Bind<TOut>(Func<TValue, Result<TOut>> next)
            {
                if (result.IsFailure)
                    return Result<TOut>.Failure(result.Error);
                                
                return next(result.Value);
            }
        }
    }
}
