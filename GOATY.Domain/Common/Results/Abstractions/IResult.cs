namespace GOATY.Domain.Common.Results.Abstractions
{
    public interface IResult
    {
        List<Error>? Errors { get; }
        bool IsSuccess { get; }
    }

    public interface IResult<T>
    {
        T Value { get; }
    }
}
