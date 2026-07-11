using RailwayResults.Errors;

namespace RailwayResults.Results
{
    public class ValidationResultBuilder<TValue> : ValidationResultBuilder
    {
        public new ValidationResultBuilder<TValue> CheckNotNull<TCheck>(TCheck value, Error error)
        {
            base.CheckNotNull(value, error);
            return this;
        }

        public new ValidationResultBuilder<TValue> CheckNotEmpty(string value, Error error)
        {
            base.CheckNotEmpty(value, error);
            return this;
        }

        public new ValidationResultBuilder<TValue> CheckNotEmpty(bool valor, Error error)
        {
            base.CheckNotEmpty(valor, error);
            return this;
        }

        /// <summary>
        /// Constrói o resultado a partir de um valor já existente.
        /// Use quando o valor de sucesso não depende de efeitos colaterais
        /// (ex: já foi montado antes ou é independente da validação).
        /// </summary>
        public new ValidationResultT<TValue> Build(TValue value)
        {
            return _errors.Count == 0
                ? ValidationResultT<TValue>.Success(value)
                : ValidationResultT<TValue>.Failure(_errors);
        }

        /// <summary>
        /// Constrói o resultado adiando a criação do valor até saber que
        /// a validação passou. Evita construir a entidade (ex: chamar seu
        /// construtor) quando os dados de entrada ainda podem ser inválidos.
        /// </summary>
        public new ValidationResultT<TValue> Build(Func<TValue> valueFactory)
        {
            return _errors.Count == 0
                ? ValidationResultT<TValue>.Success(valueFactory())
                : ValidationResultT<TValue>.Failure(_errors);
        }

        public new static ValidationResultBuilder<TValue> Create()
          => new();
    }
}
