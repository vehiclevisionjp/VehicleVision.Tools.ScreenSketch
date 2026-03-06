using VehicleVision.Tools.ScreenSketch.Rendering;

namespace VehicleVision.Tools.ScreenSketch.Tests.Rendering;

public class ColorResolverTests
{
    [Theory]
    [InlineData("#FF0000", "#FF0000")]
    [InlineData("#00ff00", "#00ff00")]
    [InlineData("#1A2B3C", "#1A2B3C")]
    public void Resolve_HexColor_ReturnsSameValue(string input, string expected)
    {
        Assert.Equal(expected, ColorResolver.Resolve(input));
    }

    [Theory]
    [InlineData("Red")]
    [InlineData("Blue")]
    [InlineData("Green")]
    [InlineData("White")]
    [InlineData("Black")]
    public void Resolve_KnownColorName_ReturnsHexFormat(string colorName)
    {
        var result = ColorResolver.Resolve(colorName);
        Assert.NotNull(result);
        Assert.StartsWith("#", result);
        Assert.Equal(7, result.Length);
    }

    [Theory]
    [InlineData("Control")]
    [InlineData("ActiveBorder")]
    [InlineData("Window")]
    public void Resolve_WinFormsKnownColor_ReturnsHexFormat(string colorName)
    {
        var result = ColorResolver.Resolve(colorName);
        Assert.NotNull(result);
        Assert.StartsWith("#", result);
    }

    [Theory]
    [InlineData("red")]
    [InlineData("RED")]
    [InlineData("Red")]
    [InlineData("rEd")]
    public void Resolve_CaseInsensitive(string colorName)
    {
        var result = ColorResolver.Resolve(colorName);
        Assert.NotNull(result);
        Assert.Equal(ColorResolver.Resolve("Red"), result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Resolve_NullOrWhitespace_ReturnsNull(string? input)
    {
        Assert.Null(ColorResolver.Resolve(input));
    }

    [Fact]
    public void Resolve_UnknownName_ReturnsNull()
    {
        Assert.Null(ColorResolver.Resolve("NotAValidColor"));
    }

    [Fact]
    public void Resolve_WithFallback_ReturnsResolvedColor()
    {
        var result = ColorResolver.Resolve("Red", "#000000");
        Assert.NotEqual("#000000", result);
    }

    [Fact]
    public void Resolve_WithFallback_UnknownName_ReturnsFallback()
    {
        Assert.Equal("#AABBCC", ColorResolver.Resolve("NotAColor", "#AABBCC"));
    }

    [Fact]
    public void Resolve_WithFallback_NullInput_ReturnsFallback()
    {
        Assert.Equal("#123456", ColorResolver.Resolve(null, "#123456"));
    }
}
