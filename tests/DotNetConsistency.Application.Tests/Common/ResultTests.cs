using DotNetConsistency.Application.Common;
using FluentAssertions;

namespace DotNetConsistency.Application.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Ok_IsSuccess_True()
    {
        var result = Result<string>.Ok("value");
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("value");
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Fail_IsSuccess_False()
    {
        var error = Error.NotFound("not found");
        var result = Result<string>.Fail(error);

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesOkResult()
    {
        Result<string> result = "implicit value";
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("implicit value");
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailResult()
    {
        Result<string> result = Error.NotFound("not found");
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void NonGenericResult_Ok_IsSuccess()
    {
        var result = Result.Ok();
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void NonGenericResult_Fail_IsNotSuccess()
    {
        var result = Result.Fail(Error.Conflict("conflict"));
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void NonGenericResult_ImplicitConversion_FromError()
    {
        Result result = Error.Unexpected("boom");
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Unexpected);
    }
}
