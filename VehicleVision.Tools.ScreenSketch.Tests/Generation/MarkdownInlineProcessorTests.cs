using VehicleVision.Tools.ScreenSketch.Generation;

namespace VehicleVision.Tools.ScreenSketch.Tests.Generation;

public class MarkdownInlineProcessorTests
{
    [Fact]
    public void Process_NoYamlScreenBlocks_ReturnsNull()
    {
        var processor = new MarkdownInlineProcessor();
        var markdown = "# Hello\n\nSome regular markdown content.\n";

        var result = processor.Process(markdown, "/tmp/images", ".images", "test");

        Assert.Null(result);
    }

    [Fact]
    public void Process_WithYamlScreenBlock_ReturnsTransformedContent()
    {
        var processor = new MarkdownInlineProcessor();
        var yaml = """
                   screen:
                     title: テスト画面

                   window:
                     title: テスト
                     width: 400
                     height: 200
                     controls:
                       - type: label
                         text: Hello
                         x: 10
                         y: 10
                   """;

        var markdown = $"# Test\n\n```yaml-screen\n{yaml}\n```\n\nMore content.";
        var imagesDir = Path.Combine(Path.GetTempPath(), $"screen-sketch-test-{Guid.NewGuid()}");

        try
        {
            var result = processor.Process(markdown, imagesDir, ".images", "test");

            Assert.NotNull(result);
            Assert.Equal(1, result.BlockCount);
            Assert.Equal(0, result.ErrorCount);
            Assert.Contains("<!-- BEGIN:yaml-screen -->", result.TransformedContent);
            Assert.Contains("<!-- END:yaml-screen -->", result.TransformedContent);
        }
        finally
        {
            if (Directory.Exists(imagesDir))
                Directory.Delete(imagesDir, true);
        }
    }

    [Fact]
    public void Restore_NoTransformedBlocks_ReturnsNull()
    {
        var markdown = "# Hello\n\nSome content.\n";

        var result = MarkdownInlineProcessor.Restore(markdown);

        Assert.Null(result);
    }

    [Fact]
    public void Restore_TransformedBlock_RestoresCodeBlock()
    {
        var yamlContent = "screen:\n  title: Test";
        var transformed = $"""
                           # Test

                           <!-- BEGIN:yaml-screen -->
                           <!-- yaml-screen
                           {yamlContent}
                           yaml-screen -->

                           ![Test](.images/test.svg)
                           <!-- END:yaml-screen -->

                           More content.
                           """;

        var result = MarkdownInlineProcessor.Restore(transformed);

        Assert.NotNull(result);
        Assert.Equal(1, result.BlockCount);
        Assert.Contains("```yaml-screen", result.RestoredContent);
        Assert.Contains(yamlContent, result.RestoredContent);
        Assert.DoesNotContain("<!-- BEGIN:yaml-screen -->", result.RestoredContent);
    }

    [Fact]
    public void Process_RoundTrip_TransformAndRestore()
    {
        var processor = new MarkdownInlineProcessor();
        var yaml = """
                   screen:
                     title: RoundTrip

                   window:
                     title: RT
                     width: 300
                     height: 200
                     controls:
                       - type: label
                         text: Test
                         x: 10
                         y: 10
                   """;

        var originalMarkdown = $"# RoundTrip\n\n```yaml-screen\n{yaml}\n```\n";
        var imagesDir = Path.Combine(Path.GetTempPath(), $"screen-sketch-test-{Guid.NewGuid()}");

        try
        {
            // Transform
            var processResult = processor.Process(originalMarkdown, imagesDir, ".images", "test");
            Assert.NotNull(processResult);

            // Restore
            var restoreResult = MarkdownInlineProcessor.Restore(processResult.TransformedContent);
            Assert.NotNull(restoreResult);

            // Should contain the original yaml-screen block
            Assert.Contains("```yaml-screen", restoreResult.RestoredContent);
            Assert.Contains(yaml, restoreResult.RestoredContent);
        }
        finally
        {
            if (Directory.Exists(imagesDir))
                Directory.Delete(imagesDir, true);
        }
    }

    [Fact]
    public void Process_WithThemeOverride_AppliesTheme()
    {
        var processor = new MarkdownInlineProcessor("dark");
        var yaml = """
                   screen:
                     title: Dark

                   window:
                     title: Dark
                     width: 300
                     height: 200
                     controls:
                       - type: label
                         text: Dark Mode
                         x: 10
                         y: 10
                   """;

        var markdown = $"```yaml-screen\n{yaml}\n```";
        var imagesDir = Path.Combine(Path.GetTempPath(), $"screen-sketch-test-{Guid.NewGuid()}");

        try
        {
            var result = processor.Process(markdown, imagesDir, ".images", "test");

            Assert.NotNull(result);
            Assert.Equal(0, result.ErrorCount);

            // Verify that SVG was generated
            Assert.NotEmpty(result.GeneratedFiles);
            Assert.True(File.Exists(result.GeneratedFiles[0]));
        }
        finally
        {
            if (Directory.Exists(imagesDir))
                Directory.Delete(imagesDir, true);
        }
    }

    [Fact]
    public void Process_EmbedInline_ContainsSvgInOutput()
    {
        var processor = new MarkdownInlineProcessor();
        var yaml = """
                   screen:
                     title: Inline

                   window:
                     title: Inline
                     width: 300
                     height: 200
                     controls:
                       - type: label
                         text: Inline
                         x: 10
                         y: 10
                   """;

        var markdown = $"```yaml-screen\n{yaml}\n```";
        var imagesDir = Path.Combine(Path.GetTempPath(), $"screen-sketch-test-{Guid.NewGuid()}");

        try
        {
            var result = processor.Process(markdown, imagesDir, ".images", "test", embedSvgInline: true);

            Assert.NotNull(result);
            Assert.Contains("<svg", result.TransformedContent);
            Assert.Contains("class=\"screen-svg\"", result.TransformedContent);
        }
        finally
        {
            if (Directory.Exists(imagesDir))
                Directory.Delete(imagesDir, true);
        }
    }
}
