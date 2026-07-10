using RailwayResults.Errors;

namespace RailwayResults.Results
{

    public class ValidationResult : Result
    {
        public IEnumerable<Error> Errors = [];

        protected ValidationResult(bool isSuccess, IEnumerable<Error> errors)
            : base(isSuccess, GetError(errors))
        {
            if (isSuccess && errors.Any())
            {
                throw new ArgumentException(
                    "Um resultado de sucesso não pode conter erros.");
            }

            Errors = errors;
        }

        public static new ValidationResult Success()
            => new(true, Error.Empty());

        public static ValidationResult Failure(IEnumerable<Error> errors)
            => new(false, errors);

        private static Error GetError(IEnumerable<Error> errors)
        {
            return errors.Any()
                ? CommonErrors.MultipleErrors
                : Error.None();
        }
    }
}
