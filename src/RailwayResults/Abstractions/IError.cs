namespace RailwayResults.Abstractions
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        Unauthorized,       
        None,
    }

    public interface IError
    {
        public string Code { get; }
        public string Message { get; }
        public ErrorType Type { get; }
    }
}
