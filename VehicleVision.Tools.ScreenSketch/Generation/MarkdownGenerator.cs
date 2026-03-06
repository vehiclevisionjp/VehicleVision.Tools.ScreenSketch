using System.Globalization;
using System.Text;
using VehicleVision.Tools.ScreenSketch.Models;

namespace VehicleVision.Tools.ScreenSketch.Generation;

/// <summary>画面定義から Markdown ドキュメントを生成する</summary>
public class MarkdownGenerator
{
    /// <summary>1 画面分の Markdown を生成</summary>
    public static string Generate(ScreenDefinition definition, string svgRelativePath)
    {
        var sb = new StringBuilder();
        var screen = definition.Screen ?? new ScreenInfo();
        var today = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        // タイトル
        sb.AppendLine(CultureInfo.InvariantCulture, $"# {screen.Title}");
        sb.AppendLine();
        sb.AppendLine(screen.Description);
        sb.AppendLine();

        // doctoc プレースホルダー
        sb.AppendLine("<!-- START doctoc generated TOC please keep comment here to allow auto update -->");
        sb.AppendLine("<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->");
        sb.AppendLine("<!-- END doctoc generated TOC please keep comment here to allow auto update -->");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // 画面イメージ
        sb.AppendLine("## 画面イメージ");
        sb.AppendLine();
        sb.AppendLine(CultureInfo.InvariantCulture, $"![{screen.Title}]({svgRelativePath})");
        sb.AppendLine();

        // コントロール説明
        if (definition.Annotations is { Count: > 0 })
        {
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## コントロール説明");
            sb.AppendLine();
            sb.AppendLine("| ラベル | 説明 |");
            sb.AppendLine("| ------ | ---- |");
            foreach (var ann in definition.Annotations)
                sb.AppendLine(CultureInfo.InvariantCulture, $"| {ann.Label} | {ann.Description} |");
            sb.AppendLine();
        }

        // コネクタ（手順）説明
        if (definition.Connectors is { Count: > 0 })
        {
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("## 手順説明");
            sb.AppendLine();
            sb.AppendLine("| ラベル | 説明 |");
            sb.AppendLine("| ------ | ---- |");
            foreach (var conn in definition.Connectors)
                sb.AppendLine(CultureInfo.InvariantCulture, $"| {conn.Label} | {conn.Description} |");
            sb.AppendLine();
        }

        // 変更履歴
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("## 変更履歴");
        sb.AppendLine();
        sb.AppendLine("| 日付       | 変更内容 |");
        sb.AppendLine("| ---------- | -------- |");
        sb.AppendLine(CultureInfo.InvariantCulture, $"| {today} | 初版作成 |");
        sb.AppendLine();

        return sb.ToString();
    }

    /// <summary>複数画面のインデックスページを生成</summary>
    public static string GenerateIndex(List<(string Title, string MdFileName)> pages)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# 操作マニュアル");
        sb.AppendLine();
        sb.AppendLine("## 目次");
        sb.AppendLine();
        for (var i = 0; i < pages.Count; i++)
        {
            var (title, fileName) = pages[i];
            sb.AppendLine(CultureInfo.InvariantCulture, $"{i + 1}. [{title}]({fileName})");
        }
        sb.AppendLine();
        return sb.ToString();
    }
}
