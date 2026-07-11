using RailwayResults.Errors;

namespace RailwayResults.Results
{
    public class ValidationResultBuilder
    {
        protected readonly List<Error> _errors = [];

        public ValidationResultBuilder CheckNotNull<TValue>(TValue value, Error error)
        {
            if (value is null)
                _errors.Add(error);

            return this;
        }

        public ValidationResultBuilder CheckNotEmpty(string value, Error error)
        {
            if (string.IsNullOrEmpty(value))
                _errors.Add(error);

            return this;
        }

        public ValidationResultBuilder CheckNotEmpty(bool valor, Error error)
        {
            if (!valor)
                _errors.Add(error);

            return this;
        }

        public  ValidationResult Build()
        {
            return _errors.Count == 0 
                ? ValidationResult.Success() 
                : ValidationResult.Failure(_errors);
        }

        public static ValidationResultBuilder Create()
            => new();
    }
}
