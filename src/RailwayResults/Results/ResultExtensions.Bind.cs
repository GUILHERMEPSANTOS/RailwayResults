using RailwayResults.Results;

namespace RailwayResult.Tests.Results
{
    public static partial class ResultExtensions
    {
        extension<TValue>(Result<TValue> result)
        {
            public Result<TOut> Bind<TOut>(Func<TValue, Result<TOut>> next)
            {
                if (result.IsFailure)
                    return Result<TOut>.Failure(result.Error);
                                
                return next(result.Value);
            }
        }
    }
}
