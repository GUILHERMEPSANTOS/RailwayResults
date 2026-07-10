using RailwayResults.Errors;

namespace RailwayResults.Results
{
    public class ValidationResultT<TValue> : Result<TValue>
    {
        public IEnumerable<Error> Errors = [];

        protected ValidationResultT(bool isSuccess, TValue value, IEnumerable<Error> errors)
            : base(isSuccess, GetError(errors), value)
        {
            if (isSuccess && errors.Any())
            {
                throw new ArgumentException(
                    "Um resultado de sucesso não pode conter erros.");
            }

            Errors = errors;
        }

        public static new ValidationResultT<TValue> Success(TValue value)
            => new(true, value,Error.Empty());
        
        public static ValidationResultT<TValue> Failure(IEnumerable<Error> errors)
            => new(false, default, errors);
        public static ValidationResultT<TValue> Create(TValue value)
            => value is null ? Failure([CommonErrors.NotFound]) : Success(value);

        public static implicit operator TValue(ValidationResultT<TValue> result) => result.Value;
        
        public static implicit operator ValidationResultT<TValue>(TValue value) => Create(value);
        private static Error GetError(IEnumerable<Error> errors)
        {
            return errors.Any()
                ? CommonErrors.MultipleErrors
                : Error.None();
        }
    }
}
