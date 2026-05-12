using DotNetArchRef.Application.Common;
using FluentAssertions;

namespace DotNetArchRef.Application.Tests.Common;

public class PagedResultTests
{
    [Theory]
    [InlineData(100, 10, 10)]
    [InlineData(101, 10, 11)]
    [InlineData(10, 10, 1)]
    [InlineData(0, 10, 0)]
    public void TotalPages_IsCalculatedCorrectly(int totalCount, int pageSize, int expectedPages)
    {
        var result = new PagedResult<int>([], totalCount, 1, pageSize);
        result.TotalPages.Should().Be(expectedPages);
    }

    [Fact]
    public void HasNext_WhenNotOnLastPage_IsTrue()
    {
        var result = new PagedResult<int>([], 30, 1, 10);
        result.HasNext.Should().BeTrue();
    }

    [Fact]
    public void HasNext_WhenOnLastPage_IsFalse()
    {
        var result = new PagedResult<int>([], 30, 3, 10);
        result.HasNext.Should().BeFalse();
    }

    [Fact]
    public void HasPrev_WhenOnFirstPage_IsFalse()
    {
        var result = new PagedResult<int>([], 30, 1, 10);
        result.HasPrev.Should().BeFalse();
    }

    [Fact]
    public void HasPrev_WhenNotOnFirstPage_IsTrue()
    {
        var result = new PagedResult<int>([], 30, 2, 10);
        result.HasPrev.Should().BeTrue();
    }
}
