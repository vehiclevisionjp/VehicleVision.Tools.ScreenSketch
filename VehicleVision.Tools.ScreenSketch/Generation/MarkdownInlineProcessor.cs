using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using VehicleVision.Tools.ScreenSketch.Models;
using VehicleVision.Tools.ScreenSketch.Rendering;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace VehicleVision.Tools.ScreenSketch.Generation;

/// <summary>
/// Markdown 内の ```yaml-screen コードブロックを検出し、
/// SVG 画像埋め込み + アノテーションテーブルに変換するプロセッサ。
/// 変換時に元の yaml-screen を HTML コメントとして保持し、再変換にも対応する。
/// </summary>
public class MarkdownInlineProcessor
{
    /// <summary>未変換の yaml-screen コードブロックを検出する正規表現</summary>
    private static readonly Regex YamlScreenBlockRegex = new(
        @"```yaml-screen\r?\n([\s\S]*?)\r?\n```",
        RegexOptions.Compiled);

    /// <summary>
    /// 変換済みの yaml-screen ブロック（BEGIN/END マーカーで囲まれたもの）を検出する正規表現。
    /// 再変換時に元の YAML を復元するために使用する。
    /// </summary>
    private static readonly Regex TransformedBlockRegex = new(
        @"<!-- BEGIN:yaml-screen -->\r?\n<!-- yaml-screen\r?\n([\s\S]*?)\r?\nyaml-screen -->\r?\n[\s\S]*?<!-- END:yaml-screen -->",
        RegexOptions.Compiled);

    private readonly IDeserializer _deserializer;
    private readonly string? _themeNameOverride;

    public MarkdownInlineProcessor(string? themeNameOverride = null)
    {
        _themeNameOverride = themeNameOverride;
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    /// <summary>
    /// Markdown ファイルを処理し、yaml-screen ブロックを SVG + テーブルに変換する。
    /// SVG ファイルは imagesDir に出力され、Markdown 内にはインライン埋め込みまたは画像参照が挿入される。
    /// 元の yaml-screen は HTML コメントとして保持され、再変換が可能。
    /// </summary>
    /// <param name="markdownContent">入力 Markdown テキスト</param>
    /// <param name="imagesDir">SVG ファイルの出力先ディレクトリ（絶対パス）</param>
    /// <param name="imageRelativeDir">Markdown から見た画像の相対パス（例: ".screen-temp"）</param>
    /// <param name="baseName">ファイル名ベース（SVG ファイル名の prefix）</param>
    /// <param name="embedSvgInline">true の場合、SVG を Markdown にインライン埋め込みする（PDF/HTML 生成向け）</param>
    /// <returns>変換後の Markdown テキスト。変換がなければ null</returns>
    public ProcessResult? Process(string markdownContent, string imagesDir,
        string imageRelativeDir, string baseName, bool embedSvgInline = false)
    {
        // 変換済みブロックをコードブロックに戻す（再変換対応）
        var preProcessed = TransformedBlockRegex.Replace(markdownContent, m =>
        {
            var yaml = m.Groups[1].Value;
            return $"```yaml-screen\n{yaml}\n```";
        });

        var matches = YamlScreenBlockRegex.Matches(preProcessed);
        if (matches.Count == 0)
            return null;

        var result = preProcessed;
        var generatedFiles = new List<string>();
        var errorCount = 0;

        for (var i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            var yamlContent = match.Groups[1].Value;
            var blockIndex = matches.Count > 1 ? $"_{i + 1}" : "";
            var svgFileName = $"{baseName}{blockIndex}.svg";

            try
            {
                var definition = _deserializer.Deserialize<ScreenDefinition>(yamlContent);
                var colors = ThemeColors.FromName(_themeNameOverride ?? definition.Screen?.Theme, definition.Screen?.CustomTheme);
                var renderer = new SvgRenderer(colors);
                var svg = renderer.Render(definition);

                // SVG ファイル出力
                Directory.CreateDirectory(imagesDir);
                var svgPath = Path.Combine(imagesDir, svgFileName);
                File.WriteAllText(svgPath, svg, Encoding.UTF8);
                generatedFiles.Add(svgPath);

                // 置換コンテンツを構築
                var replacement = BuildReplacement(
                    definition, svg, svgFileName, imageRelativeDir, embedSvgInline);

                // yaml-screen をコメントとして保持し、マーカーで囲む
                var wrappedReplacement = WrapWithComment(yamlContent, replacement);
                result = result.Replace(match.Value, wrappedReplacement);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"  Warning: yaml-screen block {i + 1} の変換に失敗: {ex.Message}");
                errorCount++;
            }
        }

        return new ProcessResult(result, generatedFiles, matches.Count, errorCount);
    }

    /// <summary>
    /// 元の yaml-screen を HTML コメントとして保持し、変換結果をマーカーで囲む。
    /// 再変換時にはマーカーを検出して元の YAML を復元できる。
    /// </summary>
    private static string WrapWithComment(string yamlContent, string replacement)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!-- BEGIN:yaml-screen -->");
        sb.AppendLine("<!-- yaml-screen");
        sb.AppendLine(yamlContent);
        sb.AppendLine("yaml-screen -->");
        sb.AppendLine();
        sb.AppendLine(replacement);
        sb.Append("<!-- END:yaml-screen -->");
        return sb.ToString();
    }

    private static string BuildReplacement(ScreenDefinition definition, string svg,
        string svgFileName, string imageRelativeDir, bool embedSvgInline)
    {
        var sb = new StringBuilder();
        var title = definition.Screen?.Title ?? "画面";

        if (embedSvgInline)
        {
            // PDF/HTML 生成時: SVG をインライン埋め込み
            sb.AppendLine(CultureInfo.InvariantCulture, $"<div class=\"screen-svg\">");
            sb.AppendLine(svg);
            sb.AppendLine("</div>");
        }
        else
        {
            // Markdown 表示時: 画像参照
            var imgPath = string.IsNullOrEmpty(imageRelativeDir)
                ? svgFileName
                : $"{imageRelativeDir}/{svgFileName}";
            sb.AppendLine(CultureInfo.InvariantCulture, $"![{title}]({imgPath})");
        }

        // アノテーションテーブル
        if (definition.Annotations is { Count: > 0 })
        {
            sb.AppendLine();
            sb.AppendLine("| ラベル | 説明 |");
            sb.AppendLine("| ------ | ---- |");
            foreach (var ann in definition.Annotations)
                sb.AppendLine(CultureInfo.InvariantCulture, $"| {ann.Label} | {ann.Description} |");
        }

        // コネクタ（手順）テーブル
        if (definition.Connectors is { Count: > 0 })
        {
            sb.AppendLine();
            sb.AppendLine("| ラベル | 説明 |");
            sb.AppendLine("| ------ | ---- |");
            foreach (var conn in definition.Connectors)
                sb.AppendLine(CultureInfo.InvariantCulture, $"| {conn.Label} | {conn.Description} |");
        }

        return sb.ToString().TrimEnd('\r', '\n');
    }

    /// <summary>
    /// 変換済みの yaml-screen ブロック（BEGIN/END マーカーで囲まれたもの）を
    /// 元の ```yaml-screen コードブロックに復元する。
    /// </summary>
    /// <param name="markdownContent">入力 Markdown テキスト</param>
    /// <returns>復元後の Markdown テキスト。変換済みブロックがなければ null</returns>
    public static RestoreResult? Restore(string markdownContent)
    {
        var matches = TransformedBlockRegex.Matches(markdownContent);
        if (matches.Count == 0)
            return null;

        var result = markdownContent;
        foreach (Match match in matches)
        {
            var yaml = match.Groups[1].Value;
            var codeBlock = $"```yaml-screen\n{yaml}\n```";
            result = result.Replace(match.Value, codeBlock);
        }

        return new RestoreResult(result, matches.Count);
    }

    public record ProcessResult(
        string TransformedContent,
        List<string> GeneratedFiles,
        int BlockCount,
        int ErrorCount);

    public record RestoreResult(
        string RestoredContent,
        int BlockCount);
}
