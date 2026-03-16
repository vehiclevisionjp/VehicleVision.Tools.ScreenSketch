using VehicleVision.Tools.ScreenSketch.Generation;
using VehicleVision.Tools.ScreenSketch.Models;

namespace VehicleVision.Tools.ScreenSketch.Tests.Generation;

public class MarkdownGeneratorTests
{
    [Fact]
    public void Generate_BasicDefinition_ContainsTitle()
    {
        var def = new ScreenDefinition
        {
            Screen = new ScreenInfo
            {
                Title = "テスト画面",
                Description = "テスト説明"
            }
        };

        var md = MarkdownGenerator.Generate(def, "test.svg");

        Assert.Contains("# テスト画面", md);
        Assert.Contains("テスト説明", md);
    }

    [Fact]
    public void Generate_ContainsSvgImageReference()
    {
        var def = new ScreenDefinition
        {
            Screen = new ScreenInfo { Title = "Sample" }
        };

        var md = MarkdownGenerator.Generate(def, "images/sample.svg");

        Assert.Contains("![Sample](images/sample.svg)", md);
    }

    [Fact]
    public void Generate_ContainsDoctocPlaceholders()
    {
        var def = new ScreenDefinition
        {
            Screen = new ScreenInfo { Title = "Test" }
        };

        var md = MarkdownGenerator.Generate(def, "test.svg");

        Assert.Contains("<!-- START doctoc", md);
        Assert.Contains("<!-- END doctoc", md);
    }

    [Fact]
    public void Generate_WithAnnotations_ContainsAnnotationTable()
    {
        var def = new ScreenDefinition
        {
            Screen = new ScreenInfo { Title = "Annotated" },
            Annotations =
            [
                new AnnotationDefinition
                {
                    Label = "①",
                    Description = "ユーザー名を入力"
                },
                new AnnotationDefinition
                {
                    Label = "②",
                    Description = "パスワードを入力"
                }
            ]
        };

        var md = MarkdownGenerator.Generate(def, "test.svg");

        Assert.Contains("## コントロール説明", md);
        Assert.Contains("| ① | ユーザー名を入力 |", md);
        Assert.Contains("| ② | パスワードを入力 |", md);
    }

    [Fact]
    public void Generate_WithoutAnnotations_NoAnnotationTable()
    {
        var def = new ScreenDefinition
        {
            Screen = new ScreenInfo { Title = "No Annotations" }
        };

        var md = MarkdownGenerator.Generate(def, "test.svg");

        Assert.DoesNotContain("## コントロール説明", md);
    }

    [Fact]
    public void Generate_WithConnectors_ContainsConnectorTable()
    {
        var def = new ScreenDefinition
        {
            Screen = new ScreenInfo { Title = "With Connectors" },
            Connectors =
            [
                new ConnectorDefinition
                {
                    Label = "①",
                    Description = "入力してから登録"
                }
            ]
        };

        var md = MarkdownGenerator.Generate(def, "test.svg");

        Assert.Contains("## 手順説明", md);
        Assert.Contains("| ① | 入力してから登録 |", md);
    }

    [Fact]
    public void Generate_ContainsChangeHistory()
    {
        var def = new ScreenDefinition
        {
            Screen = new ScreenInfo { Title = "Test" }
        };

        var md = MarkdownGenerator.Generate(def, "test.svg");

        Assert.Contains("## 変更履歴", md);
        Assert.Contains("初版作成", md);
    }

    [Fact]
    public void Generate_NullScreen_UsesDefaults()
    {
        var def = new ScreenDefinition();

        var md = MarkdownGenerator.Generate(def, "test.svg");

        Assert.NotNull(md);
        Assert.Contains("#", md);
    }

    [Fact]
    public void GenerateIndex_SinglePage_ContainsLink()
    {
        var pages = new List<(string Title, string MdFileName)>
        {
            ("ログイン画面", "login.md")
        };

        var md = MarkdownGenerator.GenerateIndex(pages);

        Assert.Contains("# 操作マニュアル", md);
        Assert.Contains("## 目次", md);
        Assert.Contains("1. [ログイン画面](login.md)", md);
    }

    [Fact]
    public void GenerateIndex_MultiplePages_NumberedCorrectly()
    {
        var pages = new List<(string Title, string MdFileName)>
        {
            ("ログイン", "login.md"),
            ("ダッシュボード", "dashboard.md"),
            ("設定", "settings.md")
        };

        var md = MarkdownGenerator.GenerateIndex(pages);

        Assert.Contains("1. [ログイン](login.md)", md);
        Assert.Contains("2. [ダッシュボード](dashboard.md)", md);
        Assert.Contains("3. [設定](settings.md)", md);
    }

    [Fact]
    public void GenerateIndex_EmptyList_ContainsHeaderOnly()
    {
        var pages = new List<(string Title, string MdFileName)>();

        var md = MarkdownGenerator.GenerateIndex(pages);

        Assert.Contains("# 操作マニュアル", md);
        Assert.Contains("## 目次", md);
    }
}
