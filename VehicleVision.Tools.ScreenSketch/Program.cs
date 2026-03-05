using VehicleVision.Tools.ScreenSketch.Generation;
using VehicleVision.Tools.ScreenSketch.Models;
using VehicleVision.Tools.ScreenSketch.Rendering;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace VehicleVision.Tools.ScreenSketch;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length < 1)
        {
            PrintUsage();
            return 1;
        }

        var command = args[0].ToLowerInvariant();

        return command switch
        {
            "generate" => RunGenerate(args.Skip(1).ToArray()),
            "transform" => RunTransform(args.Skip(1).ToArray()),
            "restore" => RunRestore(args.Skip(1).ToArray()),
            "render" => RunRender(),
            _ => RunGenerate(args) // 後方互換: コマンド指定なしの場合は generate
        };
    }

    // ────────────────────────────────────────────
    //  generate: YAML → SVG + Markdown
    // ────────────────────────────────────────────

    private static int RunGenerate(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: ManualGenerator generate <input-path> [output-dir]");
            return 1;
        }

        var inputPath = args[0];
        var outputDir = args.Length > 1 ? args[1] : "./output";

        var yamlFiles = GetYamlFiles(inputPath);
        if (yamlFiles.Count == 0)
        {
            Console.Error.WriteLine($"Error: YAML ファイルが見つかりません: {inputPath}");
            return 1;
        }

        Directory.CreateDirectory(outputDir);
        var imagesDir = Path.Combine(outputDir, "images");
        Directory.CreateDirectory(imagesDir);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        var allPages = new List<(string Title, string MdFileName)>();
        var errorCount = 0;

        foreach (var yamlFile in yamlFiles.OrderBy(f => f))
        {
            Console.WriteLine($"Processing: {yamlFile}");
            try
            {
                var yaml = File.ReadAllText(yamlFile);
                var definition = deserializer.Deserialize<ScreenDefinition>(yaml);
                var baseName = Path.GetFileNameWithoutExtension(yamlFile);

                var renderer = new SvgRenderer();
                var svg = renderer.Render(definition);
                var svgPath = Path.Combine(imagesDir, $"{baseName}.svg");
                File.WriteAllText(svgPath, svg);
                Console.WriteLine($"  -> SVG: {svgPath}");

                var md = MarkdownGenerator.Generate(definition, $"images/{baseName}.svg");
                var mdPath = Path.Combine(outputDir, $"{baseName}.md");
                File.WriteAllText(mdPath, md);
                Console.WriteLine($"  -> Markdown: {mdPath}");

                allPages.Add((definition.Screen?.Title ?? baseName, $"{baseName}.md"));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"  Error: {ex.Message}");
                errorCount++;
            }
        }

        if (allPages.Count > 1)
        {
            var indexMd = MarkdownGenerator.GenerateIndex(allPages);
            var indexPath = Path.Combine(outputDir, "index.md");
            File.WriteAllText(indexPath, indexMd);
            Console.WriteLine($"Generated index: {indexPath}");
        }

        Console.WriteLine($"Done. {allPages.Count} page(s) generated, {errorCount} error(s).");
        return errorCount > 0 ? 1 : 0;
    }

    // ────────────────────────────────────────────
    //  transform: Markdown 内の yaml-screen ブロックを SVG + テーブルに変換
    // ────────────────────────────────────────────

    private static int RunTransform(string[] args)
    {
        var inline = false;
        var inputPath = "";
        var outputDir = "";

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--inline":
                    inline = true;
                    break;
                default:
                    if (string.IsNullOrEmpty(inputPath))
                        inputPath = args[i];
                    else if (string.IsNullOrEmpty(outputDir))
                        outputDir = args[i];
                    break;
            }
        }

        if (string.IsNullOrEmpty(inputPath))
        {
            Console.Error.WriteLine("Usage: ManualGenerator transform <input-path> [output-dir] [--inline]");
            Console.Error.WriteLine();
            Console.Error.WriteLine("  <input-path>  Markdown ファイルまたはディレクトリ");
            Console.Error.WriteLine("  [output-dir]   出力先（省略時は入力と同じ場所に上書き）");
            Console.Error.WriteLine("  --inline       SVG をインライン埋め込みする（PDF/HTML 生成前処理向け）");
            return 1;
        }

        var mdFiles = GetMarkdownFiles(inputPath);
        if (mdFiles.Count == 0)
        {
            Console.Error.WriteLine($"Error: Markdown ファイルが見つかりません: {inputPath}");
            return 1;
        }

        var processor = new MarkdownInlineProcessor();
        var totalBlocks = 0;
        var totalErrors = 0;

        foreach (var mdFile in mdFiles.OrderBy(f => f))
        {
            var baseName = Path.GetFileNameWithoutExtension(mdFile);
            var sourceDir = Path.GetDirectoryName(mdFile) ?? ".";
            var destDir = string.IsNullOrEmpty(outputDir) ? sourceDir : outputDir;
            var imagesDir = Path.Combine(destDir, ".screen-temp");

            Console.WriteLine($"Processing: {mdFile}");

            var content = File.ReadAllText(mdFile);
            var result = processor.Process(content, imagesDir, ".screen-temp", baseName, inline);

            if (result == null)
            {
                Console.WriteLine("  (yaml-screen ブロックなし)");
                // 出力先が異なる場合はそのままコピー
                if (!string.IsNullOrEmpty(outputDir))
                {
                    Directory.CreateDirectory(destDir);
                    var destPath = Path.Combine(destDir, Path.GetFileName(mdFile));
                    File.Copy(mdFile, destPath, overwrite: true);
                }
                continue;
            }

            totalBlocks += result.BlockCount;
            totalErrors += result.ErrorCount;

            foreach (var f in result.GeneratedFiles)
                Console.WriteLine($"  -> SVG: {f}");

            // 変換結果を書き出し
            Directory.CreateDirectory(destDir);
            var outputPath = Path.Combine(destDir, Path.GetFileName(mdFile));
            File.WriteAllText(outputPath, result.TransformedContent);
            Console.WriteLine($"  -> Markdown: {outputPath} ({result.BlockCount} block(s))");
        }

        Console.WriteLine($"Done. {totalBlocks} block(s) transformed, {totalErrors} error(s).");
        return totalErrors > 0 ? 1 : 0;
    }

    // ────────────────────────────────────────────
    //  restore: 変換済み yaml-screen をコードブロックに復元
    // ────────────────────────────────────────────

    private static int RunRestore(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: ManualGenerator restore <input-path>");
            Console.Error.WriteLine();
            Console.Error.WriteLine("  <input-path>  Markdown ファイルまたはディレクトリ");
            return 1;
        }

        var inputPath = args[0];
        var mdFiles = GetMarkdownFiles(inputPath);
        if (mdFiles.Count == 0)
        {
            Console.Error.WriteLine($"Error: Markdown ファイルが見つかりません: {inputPath}");
            return 1;
        }

        var totalBlocks = 0;

        foreach (var mdFile in mdFiles.OrderBy(f => f))
        {
            Console.WriteLine($"Processing: {mdFile}");

            var content = File.ReadAllText(mdFile);
            var result = MarkdownInlineProcessor.Restore(content);

            if (result == null)
            {
                Console.WriteLine("  (変換済み yaml-screen ブロックなし)");
                continue;
            }

            totalBlocks += result.BlockCount;
            File.WriteAllText(mdFile, result.RestoredContent);
            Console.WriteLine($"  -> Restored: {mdFile} ({result.BlockCount} block(s))");
        }

        Console.WriteLine($"Done. {totalBlocks} block(s) restored.");
        return 0;
    }

    // ────────────────────────────────────────────
    //  render: stdin YAML → stdout SVG
    // ────────────────────────────────────────────

    private static int RunRender()
    {
        try
        {
            var yaml = Console.In.ReadToEnd();

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            var definition = deserializer.Deserialize<ScreenDefinition>(yaml);
            var renderer = new SvgRenderer();
            var svg = renderer.Render(definition);

            Console.Write(svg);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    // ────────────────────────────────────────────
    //  ファイル探索
    // ────────────────────────────────────────────

    private static List<string> GetYamlFiles(string path)
    {
        if (File.Exists(path))
            return [path];

        if (Directory.Exists(path))
            return Directory.GetFiles(path, "*.yaml", SearchOption.TopDirectoryOnly)
                .Concat(Directory.GetFiles(path, "*.yml", SearchOption.TopDirectoryOnly))
                .ToList();

        return [];
    }

    private static List<string> GetMarkdownFiles(string path)
    {
        if (File.Exists(path) && path.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            return [path];

        if (Directory.Exists(path))
            return Directory.GetFiles(path, "*.md", SearchOption.AllDirectories)
                .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}.template{Path.DirectorySeparatorChar}"))
                .ToList();

        return [];
    }

    private static void PrintUsage()
    {
        Console.Error.WriteLine("VehicleVision.Dealer.Tools.ManualGenerator");
        Console.Error.WriteLine();
        Console.Error.WriteLine("Commands:");
        Console.Error.WriteLine();
        Console.Error.WriteLine("  generate <input-path> [output-dir]");
        Console.Error.WriteLine("    YAML ファイルから SVG + Markdown を新規生成する");
        Console.Error.WriteLine();
        Console.Error.WriteLine("  transform <input-path> [output-dir] [--inline]");
        Console.Error.WriteLine("    Markdown 内の ```yaml-screen ブロックを SVG + テーブルに変換する");
        Console.Error.WriteLine("    --inline: SVG をインライン埋め込み（PDF/HTML 生成前処理向け）");
        Console.Error.WriteLine();
        Console.Error.WriteLine("  restore <input-path>");
        Console.Error.WriteLine("    変換済みの yaml-screen ブロックをコードブロックに復元する");
        Console.Error.WriteLine();
        Console.Error.WriteLine("  render");
        Console.Error.WriteLine("    stdin から YAML を読み取り、stdout に SVG を出力する");
        Console.Error.WriteLine();
        Console.Error.WriteLine("Examples:");
        Console.Error.WriteLine("  ManualGenerator generate screens/ docs/manual/");
        Console.Error.WriteLine("  ManualGenerator transform docs/ docs/_output/");
        Console.Error.WriteLine("  ManualGenerator transform docs/ docs/_temp/ --inline");
        Console.Error.WriteLine("  ManualGenerator restore docs/");
        Console.Error.WriteLine("  echo 'window: ...' | ManualGenerator render");
    }
}
