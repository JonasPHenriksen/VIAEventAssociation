namespace VIAEventAssociation.Core.Tools.OperationResult
{
    public class Error
    {
        public string Code { get; }
        public string Message { get; }
        public string? Type { get; }

        public Error(string code, string message, string? type = null)
        {
            Code = code;
            Message = message;
            Type = type;
        }
    }

    public sealed class Unit
    {
        public static readonly Unit Value = new();
        private Unit() { }
    }

    public class OperationResult<T>
    {
        public T? Value { get; }
        public bool IsSuccess => Errors.Count == 0;
        public List<Error> Errors { get; } = new();

        private OperationResult(T value)
        {
            Value = value;
        }

        private OperationResult(List<Error> errors)
        {
            Errors = errors;
        }

        public static OperationResult<T> Success(T value) => new(value);
        public static OperationResult<T> Failure(List<Error> errors) => new(errors);
        public static OperationResult<T> Failure(string code, string message, string? type = null) =>
            new(new List<Error> { new(code, message, type) });

        public static OperationResult<Unit> Success() => new(Unit.Value);
        public static OperationResult<T> Fail(params Error[] errors) => new(errors.ToList());
        public static OperationResult<T> Fail(string code, string message, string? type = null) =>
            new(new List<Error> { new(code, message, type) });

        public static OperationResult<T> Combine(params OperationResult<T>[] results)
        {
            var errors = results.SelectMany(r => r.Errors).ToList();
            return errors.Count > 0 ? Failure(errors) : Success(results.Last().Value!);
        }

        public static implicit operator OperationResult<T>(T value) => Success(value);
        public static implicit operator OperationResult<T>(Error error) => Failure(error.Code, error.Message, error.Type);
        public static implicit operator OperationResult<T>(List<Error> errors) => Failure(errors);
    }

}