using System.Drawing;

namespace VehicleVision.Tools.ScreenSketch.Rendering;

/// <summary>
/// 色文字列を SVG 用の #RRGGBB 形式に解決するユーティリティ。
/// "#FF0000" のような16進指定に加え、WinForms で使われる
/// <see cref="KnownColor"/> 名（例: "Control", "ActiveBorder", "Window"）をサポートする。
/// </summary>
public static class ColorResolver
{
    /// <summary>WinForms 系カラー名 → #RRGGBB のマッピング（大文字小文字を区別しない）</summary>
    private static readonly Dictionary<string, string> NamedColors = BuildNamedColorMap();

    /// <summary>
    /// 色文字列を #RRGGBB 形式に解決する。
    /// "#" で始まる場合はそのまま返し、それ以外は WinForms の <see cref="KnownColor"/> 名として解決を試みる。
    /// 解決できない場合は null を返す。
    /// </summary>
    public static string? Resolve(string? colorValue)
    {
        if (string.IsNullOrWhiteSpace(colorValue))
            return null;

        // 既に #RRGGBB 形式の場合はそのまま返す
        if (colorValue.StartsWith('#'))
            return colorValue;

        // WinForms カラー名で解決
        if (NamedColors.TryGetValue(colorValue, out var hex))
            return hex;

        return null;
    }

    /// <summary>
    /// 色文字列を #RRGGBB 形式に解決する。解決できない場合は fallback を返す。
    /// </summary>
    public static string Resolve(string? colorValue, string fallback)
    {
        return Resolve(colorValue) ?? fallback;
    }

    /// <summary>KnownColor 列挙体からカラーマップを構築する</summary>
    private static Dictionary<string, string> BuildNamedColorMap()
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (KnownColor kc in Enum.GetValues<KnownColor>())
        {
            var color = Color.FromKnownColor(kc);
            // A=0 の名前付きカラー（Transparent 等）も登録する。
            // SVG は #RRGGBB 形式のため透明度（Alpha）は無視される。
            map.TryAdd(kc.ToString(), $"#{color.R:X2}{color.G:X2}{color.B:X2}");
        }

        return map;
    }
}
