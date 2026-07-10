using RailwayResults.Abstractions;
using RailwayResults.Errors;

namespace RailwayResults.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error is { Type: not ErrorType.None })
            {
                throw new ArgumentException("Um resultado de sucesso não pode conter um erro.");
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success()
            => new(true, Error.None());
        
        public static Result Failure(Error error)
            => new(false, error);        
    }
}
