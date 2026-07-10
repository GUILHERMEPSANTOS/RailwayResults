using RailwayResults.Errors;
namespace RailwayResults.Results
{
    public class Result<TValue> : Result
    {
        public TValue Value { get; }

        protected Result(bool isSuccess, Error error, TValue value)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<TValue> Success(TValue value)
            => new(true, Error.None(), value);

        public static Result<TValue> Failure(Error error)
            => new(false, error, default);

        public static Result<TValue> Create(TValue value)
            => value is null ? Failure(CommonErrors.NotFound) : Success(value);

        public static implicit operator TValue(Result<TValue> result) => result.Value;
        public static implicit operator Result<TValue>(TValue value) => Create(value);
    }
}
