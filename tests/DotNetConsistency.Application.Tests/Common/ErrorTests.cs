using DotNetConsistency.Application.Common;
using FluentAssertions;

namespace DotNetConsistency.Application.Tests.Common;

public class ErrorTests
{
    [Fact]
    public void NotFound_SetsCorrectTypeAndMessage()
    {
        var error = Error.NotFound("Book not found.");
        error.Type.Should().Be(ErrorType.NotFound);
        error.Message.Should().Be("Book not found.");
        error.Details.Should().BeNull();
    }

    [Fact]
    public void Conflict_SetsCorrectType()
    {
        var error = Error.Conflict("Already exists.");
        error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void Validation_SetsDetailsCorrectly()
    {
        var details = new[] { "Title is required.", "Price must be positive." };
        var error = Error.Validation("Validation failed.", details);

        error.Type.Should().Be(ErrorType.Validation);
        error.Details.Should().BeEquivalentTo(details);
    }

    [Fact]
    public void Unexpected_SetsCorrectType()
    {
        var error = Error.Unexpected("Something went wrong.");
        error.Type.Should().Be(ErrorType.Unexpected);
    }

    [Fact]
    public void TwoErrors_WithSameValues_AreEqual()
    {
        var a = Error.NotFound("not found");
        var b = Error.NotFound("not found");
        a.Should().Be(b);
    }
}
