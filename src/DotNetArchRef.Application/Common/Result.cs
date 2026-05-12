using System.Text.Json.Serialization;

namespace DotNetArchRef.Application.Common;

public sealed class Result<T>
{
    [JsonIgnore]
    public bool IsSuccess { get; }
    public T? Value { get; }
    [JsonIgnore]
    public Error? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result<T> Ok(T value) => new(value);
    public static Result<T> Fail(Error error) => new(error);

    public static implicit operator Result<T>(T value) => Ok(value);
    public static implicit operator Result<T>(Error error) => Fail(error);
}

public sealed class Result
{
    [JsonIgnore]
    public bool IsSuccess { get; }
    [JsonIgnore]
    public Error? Error { get; }

    private Result()
    {
        IsSuccess = true;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result Ok() => new();
    public static Result Fail(Error error) => new(error);

    public static implicit operator Result(Error error) => Fail(error);
}