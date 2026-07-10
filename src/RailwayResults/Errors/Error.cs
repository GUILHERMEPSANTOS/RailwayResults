using RailwayResults.Abstractions;

namespace RailwayResults.Errors
{
    public sealed class Error : IError
    {
        public string Code { get; }
        public string Message { get; }
        public ErrorType Type { get; }

        private Error(string code, string message, ErrorType type)
        {
            if (type is ErrorType.None
                    && (!string.IsNullOrEmpty(code)
                    || !string.IsNullOrEmpty(message)))

            {
                throw new ArgumentException("ErrorType.None não pode possuir código ou mensagem.");
            }

            Code = code;
            Message = message;
            Type = type;
        }

        public static Error Validation(string code, string message)
              => new(code, message, ErrorType.Validation);
        public static Error NotFound(string code, string message)
              => new(code, message, ErrorType.NotFound);
        public static Error Conflict(string code, string message)
              => new(code, message, ErrorType.Conflict);
        public static Error Unauthorized(string code, string message)
              => new(code, message, ErrorType.Unauthorized);
        public static Error None()
              => new(string.Empty, string.Empty, ErrorType.None);
        public static IEnumerable<Error> Empty()
           => [];
    }
}
