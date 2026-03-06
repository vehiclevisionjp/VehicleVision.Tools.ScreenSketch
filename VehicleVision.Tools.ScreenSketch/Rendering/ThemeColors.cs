using System.Reflection;
using System.Text.RegularExpressions;

namespace VehicleVision.Tools.ScreenSketch.Rendering;

/// <summary>SVG 描画で使用する色定義。テーマごとにインスタンスを生成する</summary>
public partial class ThemeColors
{
    // ── Window ──
    public string WindowBackground { get; init; } = "#F0F0F0";
    public string WindowBorder { get; init; } = "#999999";
    public string TitleBarBackground { get; init; } = "#0078D4";
    public string TitleBarText { get; init; } = "#FFFFFF";

    // ── Button ──
    public string ButtonBackground { get; init; } = "#E1E1E1";
    public string ButtonBorder { get; init; } = "#ADADAD";
    public string ButtonText { get; init; } = "#1E1E1E";

    // ── TextBox ──
    public string TextBoxBackground { get; init; } = "#FFFFFF";
    public string TextBoxBorder { get; init; } = "#7A7A7A";
    public string TextBoxText { get; init; } = "#1E1E1E";
    public string TextBoxPlaceholder { get; init; } = "#A0A0A0";

    // ── GroupBox ──
    public string GroupBorder { get; init; } = "#D5DFE5";
    public string GroupText { get; init; } = "#333333";

    // ── DataGrid ──
    public string GridBackground { get; init; } = "#FFFFFF";
    public string GridHeaderBackground { get; init; } = "#F5F5F5";
    public string GridBorder { get; init; } = "#D5DFE5";
    public string GridHeaderText { get; init; } = "#1E1E1E";
    public string GridCellText { get; init; } = "#333333";
    public string GridRowAlternate { get; init; } = "#FAFAFA";

    // ── MenuBar ──
    public string MenuBackground { get; init; } = "#F5F5F5";
    public string MenuText { get; init; } = "#1E1E1E";
    public string MenuBorder { get; init; } = "#D5DFE5";

    // ── StatusBar ──
    public string StatusBarBackground { get; init; } = "#007ACC";
    public string StatusBarText { get; init; } = "#FFFFFF";

    // ── Label ──
    public string LabelText { get; init; } = "#1E1E1E";

    // ── LinkLabel ──
    public string LinkLabelText { get; init; } = "#0066CC";
    public string LinkLabelUnderline { get; init; } = "#0066CC";

    // ── TreeView ──
    public string TreeViewBackground { get; init; } = "#FFFFFF";
    public string TreeViewText { get; init; } = "#1E1E1E";
    public string TreeViewExpander { get; init; } = "#666666";

    // ── Toolbar ──
    public string ToolbarBackground { get; init; } = "#F0F0F0";
    public string ToolbarBorder { get; init; } = "#D5DFE5";
    public string ToolbarButtonHover { get; init; } = "#E5E5E5";
    public string ToolbarText { get; init; } = "#1E1E1E";
    public string ToolbarSeparator { get; init; } = "#C0C0C0";

    // ── Annotation ──
    public string AnnotationCircle { get; init; } = "#E53935";
    public string AnnotationText { get; init; } = "#FFFFFF";
    public string AnnotationLine { get; init; } = "#E53935";

    // ── Connector ──
    public string ConnectorLine { get; init; } = "#1976D2";
    public string ConnectorCircle { get; init; } = "#1976D2";
    public string ConnectorText { get; init; } = "#FFFFFF";

    // ── Chromeless ──
    public string ChromelessBorder { get; init; } = "#D0D0D0";

    // ── Canvas Background ──
    public string CanvasBackground { get; init; } = "#FFFFFF";

    /// <summary>テーマ名からプリセットを取得する</summary>
    /// <param name="themeName">テーマ名（default, dark, blueprint, custom）</param>
    /// <param name="customOverrides">カスタムテーマの色定義。theme が "custom" の場合に使用</param>
    public static ThemeColors FromName(string? themeName, Dictionary<string, string>? customOverrides = null)
    {
        return (themeName?.ToLowerInvariant()) switch
        {
            "dark" => CreateDark(),
            "blueprint" => CreateBlueprint(),
            "custom" => CreateCustom(customOverrides),
            _ => new ThemeColors()
        };
    }

    /// <summary>カスタムテーマ。未指定の要素は標準テーマにフォールバックする</summary>
    public static ThemeColors CreateCustom(Dictionary<string, string>? overrides)
    {
        var colors = new ThemeColors();
        if (overrides is not { Count: > 0 }) return colors;

        foreach (var (key, value) in overrides)
        {
            if (ColorPropertyMap.TryGetValue(key, out var prop) && HexColorRegex().IsMatch(value))
            {
                prop.SetValue(colors, value);
            }
        }

        return colors;
    }

    /// <summary>色プロパティのキャッシュ（大文字小文字を区別しないキー）</summary>
    private static readonly Dictionary<string, PropertyInfo> ColorPropertyMap =
        typeof(ThemeColors).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(string) && p.CanWrite)
            .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

    [GeneratedRegex(@"^#[0-9A-Fa-f]{6}$")]
    private static partial Regex HexColorRegex();

    /// <summary>ダークテーマ</summary>
    public static ThemeColors CreateDark() => new()
    {
        CanvasBackground = "#1E1E1E",
        WindowBackground = "#2D2D2D",
        WindowBorder = "#555555",
        TitleBarBackground = "#3C3C3C",
        TitleBarText = "#CCCCCC",
        ButtonBackground = "#3C3C3C",
        ButtonBorder = "#555555",
        ButtonText = "#CCCCCC",
        TextBoxBackground = "#3C3C3C",
        TextBoxBorder = "#555555",
        TextBoxText = "#CCCCCC",
        TextBoxPlaceholder = "#808080",
        GroupBorder = "#555555",
        GroupText = "#CCCCCC",
        GridBackground = "#2D2D2D",
        GridHeaderBackground = "#383838",
        GridBorder = "#555555",
        GridHeaderText = "#CCCCCC",
        GridCellText = "#BBBBBB",
        GridRowAlternate = "#333333",
        MenuBackground = "#2D2D2D",
        MenuText = "#CCCCCC",
        MenuBorder = "#555555",
        StatusBarBackground = "#007ACC",
        StatusBarText = "#FFFFFF",
        LabelText = "#CCCCCC",
        LinkLabelText = "#4FC1FF",
        LinkLabelUnderline = "#4FC1FF",
        TreeViewBackground = "#2D2D2D",
        TreeViewText = "#CCCCCC",
        TreeViewExpander = "#AAAAAA",
        ToolbarBackground = "#2D2D2D",
        ToolbarBorder = "#555555",
        ToolbarButtonHover = "#454545",
        ToolbarText = "#CCCCCC",
        ToolbarSeparator = "#555555",
        AnnotationCircle = "#E53935",
        AnnotationText = "#FFFFFF",
        AnnotationLine = "#E53935",
        ConnectorLine = "#42A5F5",
        ConnectorCircle = "#42A5F5",
        ConnectorText = "#FFFFFF",
        ChromelessBorder = "#555555"
    };

    /// <summary>設計図風ブループリントテーマ</summary>
    public static ThemeColors CreateBlueprint() => new()
    {
        CanvasBackground = "#1A3A5C",
        WindowBackground = "#1A3A5C",
        WindowBorder = "#4A90D9",
        TitleBarBackground = "#2E6EB5",
        TitleBarText = "#FFFFFF",
        ButtonBackground = "#1A3A5C",
        ButtonBorder = "#4A90D9",
        ButtonText = "#FFFFFF",
        TextBoxBackground = "#0F2A45",
        TextBoxBorder = "#4A90D9",
        TextBoxText = "#FFFFFF",
        TextBoxPlaceholder = "#7BAAD4",
        GroupBorder = "#4A90D9",
        GroupText = "#C0DEFF",
        GridBackground = "#0F2A45",
        GridHeaderBackground = "#1A3A5C",
        GridBorder = "#4A90D9",
        GridHeaderText = "#FFFFFF",
        GridCellText = "#C0DEFF",
        GridRowAlternate = "#15304F",
        MenuBackground = "#1A3A5C",
        MenuText = "#FFFFFF",
        MenuBorder = "#4A90D9",
        StatusBarBackground = "#2E6EB5",
        StatusBarText = "#FFFFFF",
        LabelText = "#FFFFFF",
        LinkLabelText = "#7EC8E3",
        LinkLabelUnderline = "#7EC8E3",
        TreeViewBackground = "#0F2A45",
        TreeViewText = "#FFFFFF",
        TreeViewExpander = "#7BAAD4",
        ToolbarBackground = "#1A3A5C",
        ToolbarBorder = "#4A90D9",
        ToolbarButtonHover = "#2E6EB5",
        ToolbarText = "#FFFFFF",
        ToolbarSeparator = "#4A90D9",
        AnnotationCircle = "#FF6B35",
        AnnotationText = "#FFFFFF",
        AnnotationLine = "#FF6B35",
        ConnectorLine = "#7EC8E3",
        ConnectorCircle = "#7EC8E3",
        ConnectorText = "#FFFFFF",
        ChromelessBorder = "#4A90D9"
    };
}
