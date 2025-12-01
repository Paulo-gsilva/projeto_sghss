using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using SGHSS.Api.Validators;

namespace Company.TestProject1;

[ExcludeFromCodeCoverage]
public class CpfAttributeTests
{
    [Theory]
    [InlineData("111.444.777-35", true)]
    [InlineData("11144477735", true)]
    [InlineData("00000000000", false)]
    [InlineData("11111111111", false)]
    [InlineData("12345678900", false)]
    [InlineData("11144477736", false)]
    [InlineData("1114447773", false)]
    public void CpfAttribute_ShouldValidateCorrectly(string cpf, bool expectedValid)
    {
        CpfAttribute attribute = new CpfAttribute();

        bool result = attribute.IsValid(cpf);

        result.Should().Be(expectedValid);
    }

    [Fact]
    public void CpfAttribute_ShouldFailOnNullOrEmpty()
    {
        CpfAttribute attribute = new CpfAttribute();

        bool nullResult = attribute.IsValid(null);
        bool emptyResult = attribute.IsValid(string.Empty);

        nullResult.Should().BeFalse();
        emptyResult.Should().BeFalse();
    }
}
