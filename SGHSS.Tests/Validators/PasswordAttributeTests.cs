using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using SGHSS.Api.Validators;

namespace SGHSS.Tests.Validators;

[ExcludeFromCodeCoverage]
public class PasswordAttributeTests
{
    [Theory]
    [InlineData("Aa1!aaaa", true)]
    [InlineData("Aa1!Aa1!", true)]
    [InlineData("aaaaaaa", false)]
    [InlineData("AAAAAAA1!", false)]
    [InlineData("aaaaaaa1!", false)]
    [InlineData("AAAAAAA!", false)]
    [InlineData("Aa1aaaa", false)]
    public void PasswordAttribute_ShouldValidateCorrectly(string senha, bool expectedValid)
    {
        PasswordAttribute attribute = new PasswordAttribute(8);

        bool result = attribute.IsValid(senha);

        result.Should().Be(expectedValid);
    }

    [Fact]
    public void PasswordAttribute_ShouldFailOnNullOrEmpty()
    {
        PasswordAttribute attribute = new PasswordAttribute();

        bool nullResult = attribute.IsValid(null);
        bool emptyResult = attribute.IsValid(string.Empty);

        nullResult.Should().BeFalse();
        emptyResult.Should().BeFalse();
    }
}