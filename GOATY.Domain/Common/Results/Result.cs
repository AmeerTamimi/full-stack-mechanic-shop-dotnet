using GOATY.Domain.Common.Results.Abstractions;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GOATY.Domain.Common.Results
{
    public class Result
    {
        public static Success Success => default;
        public static Created Created => default;
        public static Deleted Deleted => default;
        public static Updated Updated => default;
    }
    public class Result<T> : IResult<T>
    {
        [JsonConstructor]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("For serializer only.", true)]
        public Result(T? value, List<Error>? errors, bool isSuccess)
        {
            if (isSuccess)
            {
                _value = value ?? throw new ArgumentNullException(nameof(value));
                _errors = [];
                IsSuccess = true;
            }
            else
            {
                if (errors == null || errors.Count == 0)
                {
                    throw new ArgumentException("Provide at least one error.", nameof(errors));
                }

                _errors = errors;
                _value = default!;
                IsSuccess = false;
            }
        }
        public bool IsSuccess { get; }
        private readonly List<Error>? _errors = null;
        private readonly T? _value = default;

        private Result(T value)
        {
            _value = value;
            IsSuccess = true;
        }
        private Result(Error error)
        {
            _errors = [error];
            IsSuccess = false;
        }
        private Result(List<Error> errors)
        {
            _errors = errors;
            IsSuccess = false;
        }
        public static implicit operator Result<T>(T value) => new(value);
        public static implicit operator Result<T>(Error error) => new(error);
        public static implicit operator Result<T>(List<Error> errors) => new(errors);

        public T Value => IsSuccess ? _value! : default!;
        public List<Error> Errors => !IsSuccess ? _errors! : [];
        public Error Error => Errors.Count() > 0 ? Errors[0] : null!;

        public TNext Match<TNext>(Func<T, TNext> OnValue, Func<List<Error>, TNext> OnError)
            => IsSuccess ? OnValue(Value) : OnError(Errors!);
    }
}
public readonly record struct Success;
public readonly record struct Created;
public readonly record struct Deleted;
public readonly record struct Updated;