namespace RailwayResults.Errors;

public static class CommonErrors
{
    public static readonly Error None =
        Error.None();

    public static readonly Error NullValue =
        Error.Validation(
            "General.NullValue",
            "O valor informado não pode ser nulo.");

    public static readonly Error InvalidValue =
        Error.Validation(
            "General.InvalidValue",
            "O valor informado é inválido.");

    public static readonly Error NotFound =
        Error.NotFound(
            "General.NotFound",
            "O recurso solicitado não foi encontrado.");

    public static readonly Error Unauthorized =
        Error.Unauthorized(
            "General.Unauthorized",
            "O usuário não possui permissão para executar esta operação.");

    public static readonly Error Conflict =
        Error.Conflict(
            "General.Conflict",
            "A operação não pode ser concluída devido a um conflito de estado.");

    public static readonly Error Unexpected =
        Error.Conflict(
            "General.Unexpected",
            "Ocorreu um erro inesperado.");
}