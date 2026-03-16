using VehicleVision.Tools.ScreenSketch.Rendering;

namespace VehicleVision.Tools.ScreenSketch.Tests.Rendering;

public class ThemeColorsTests
{
    [Fact]
    public void FromName_Default_ReturnsDefaultTheme()
    {
        var colors = ThemeColors.FromName(null);
        Assert.Equal("#F0F0F0", colors.WindowBackground);
        Assert.Equal("#0078D4", colors.TitleBarBackground);
        Assert.Equal("#FFFFFF", colors.CanvasBackground);
    }

    [Fact]
    public void FromName_UnknownTheme_ReturnsDefaultTheme()
    {
        var colors = ThemeColors.FromName("unknown");
        Assert.Equal("#F0F0F0", colors.WindowBackground);
    }

    [Fact]
    public void FromName_Dark_ReturnsDarkTheme()
    {
        var colors = ThemeColors.FromName("dark");
        Assert.Equal("#2D2D2D", colors.WindowBackground);
        Assert.Equal("#1E1E1E", colors.CanvasBackground);
        Assert.Equal("#3C3C3C", colors.TitleBarBackground);
    }

    [Fact]
    public void FromName_Dark_CaseInsensitive()
    {
        var colors = ThemeColors.FromName("DARK");
        Assert.Equal("#2D2D2D", colors.WindowBackground);
    }

    [Fact]
    public void FromName_Blueprint_ReturnsBlueprintTheme()
    {
        var colors = ThemeColors.FromName("blueprint");
        Assert.Equal("#1A3A5C", colors.WindowBackground);
        Assert.Equal("#1A3A5C", colors.CanvasBackground);
        Assert.Equal("#4A90D9", colors.WindowBorder);
    }

    [Fact]
    public void FromName_Custom_WithOverrides_AppliesOverrides()
    {
        var overrides = new Dictionary<string, string>
        {
            ["WindowBackground"] = "#AABBCC",
            ["TitleBarBackground"] = "#112233"
        };

        var colors = ThemeColors.FromName("custom", overrides);
        Assert.Equal("#AABBCC", colors.WindowBackground);
        Assert.Equal("#112233", colors.TitleBarBackground);
        // Non-overridden should fall back to defaults
        Assert.Equal("#FFFFFF", colors.CanvasBackground);
    }

    [Fact]
    public void FromName_Custom_WithoutOverrides_ReturnsDefaultTheme()
    {
        var colors = ThemeColors.FromName("custom");
        Assert.Equal("#F0F0F0", colors.WindowBackground);
    }

    [Fact]
    public void CreateCustom_NullOverrides_ReturnsDefaultTheme()
    {
        var colors = ThemeColors.CreateCustom(null);
        Assert.Equal("#F0F0F0", colors.WindowBackground);
    }

    [Fact]
    public void CreateCustom_EmptyOverrides_ReturnsDefaultTheme()
    {
        var colors = ThemeColors.CreateCustom([]);
        Assert.Equal("#F0F0F0", colors.WindowBackground);
    }

    [Fact]
    public void CreateCustom_CaseInsensitiveKeys()
    {
        var overrides = new Dictionary<string, string>
        {
            ["windowbackground"] = "#AABBCC"
        };

        var colors = ThemeColors.CreateCustom(overrides);
        Assert.Equal("#AABBCC", colors.WindowBackground);
    }

    [Fact]
    public void CreateCustom_NamedColor_ResolvesToHex()
    {
        var overrides = new Dictionary<string, string>
        {
            ["WindowBackground"] = "Red"
        };

        var colors = ThemeColors.CreateCustom(overrides);
        Assert.StartsWith("#", colors.WindowBackground);
        Assert.NotEqual("#F0F0F0", colors.WindowBackground);
    }

    [Fact]
    public void CreateCustom_InvalidColor_KeepsDefault()
    {
        var overrides = new Dictionary<string, string>
        {
            ["WindowBackground"] = "NotAValidColor"
        };

        var colors = ThemeColors.CreateCustom(overrides);
        Assert.Equal("#F0F0F0", colors.WindowBackground);
    }

    [Fact]
    public void CreateCustom_UnknownProperty_Ignored()
    {
        var overrides = new Dictionary<string, string>
        {
            ["NonExistentProperty"] = "#AABBCC"
        };

        // Should not throw
        var colors = ThemeColors.CreateCustom(overrides);
        Assert.Equal("#F0F0F0", colors.WindowBackground);
    }

    [Fact]
    public void CreateDark_HasAllAnnotationColors()
    {
        var dark = ThemeColors.CreateDark();
        Assert.NotNull(dark.AnnotationCircle);
        Assert.NotNull(dark.AnnotationText);
        Assert.NotNull(dark.AnnotationLine);
    }

    [Fact]
    public void CreateBlueprint_HasAllConnectorColors()
    {
        var blueprint = ThemeColors.CreateBlueprint();
        Assert.NotNull(blueprint.ConnectorLine);
        Assert.NotNull(blueprint.ConnectorCircle);
        Assert.NotNull(blueprint.ConnectorText);
    }
}
