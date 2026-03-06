using VehicleVision.Tools.ScreenSketch.Models;
using VehicleVision.Tools.ScreenSketch.Rendering;

namespace VehicleVision.Tools.ScreenSketch.Tests.Rendering;

public class SvgRendererTests
{
    [Fact]
    public void Render_MinimalDefinition_ReturnsValidSvg()
    {
        var def = new ScreenDefinition
        {
            Window = new WindowDefinition
            {
                Title = "Test",
                Width = 400,
                Height = 200,
                Controls =
                [
                    new ControlDefinition { Type = "label", Text = "Hello", X = 10, Y = 10 }
                ]
            }
        };

        var renderer = new SvgRenderer();
        var svg = renderer.Render(def);

        Assert.NotNull(svg);
        Assert.Contains("<svg", svg);
        Assert.Contains("</svg>", svg);
        Assert.Contains("Hello", svg);
    }

    [Fact]
    public void Render_EmptyDefinition_ReturnsValidSvg()
    {
        var def = new ScreenDefinition();
        var renderer = new SvgRenderer();
        var svg = renderer.Render(def);

        Assert.NotNull(svg);
        Assert.Contains("<svg", svg);
        Assert.Contains("</svg>", svg);
    }

    [Fact]
    public void Render_WithTheme_UsesThemeColors()
    {
        var def = new ScreenDefinition
        {
            Window = new WindowDefinition
            {
                Title = "Dark Test",
                Width = 400,
                Height = 200
            }
        };

        var darkColors = ThemeColors.CreateDark();
        var renderer = new SvgRenderer(darkColors);
        var svg = renderer.Render(def);

        Assert.Contains(darkColors.WindowBackground, svg);
    }

    [Fact]
    public void Render_ChromelessWindow_ReturnsValidSvg()
    {
        var def = new ScreenDefinition
        {
            Window = new WindowDefinition
            {
                Title = "Chromeless",
                Width = 400,
                Height = 200,
                Chrome = false,
                Controls =
                [
                    new ControlDefinition { Type = "label", Text = "Content", X = 10, Y = 10 }
                ]
            }
        };

        var renderer = new SvgRenderer();
        var svg = renderer.Render(def);

        Assert.NotNull(svg);
        Assert.Contains("<svg", svg);
    }

    [Theory]
    [InlineData("button", "Click Me")]
    [InlineData("label", "Label Text")]
    [InlineData("textbox", null)]
    [InlineData("checkbox", "Check")]
    [InlineData("radiobutton", "Radio")]
    [InlineData("combobox", null)]
    [InlineData("progressbar", null)]
    public void Render_ControlTypes_ReturnsValidSvg(string controlType, string? text)
    {
        var def = new ScreenDefinition
        {
            Window = new WindowDefinition
            {
                Title = "Control Test",
                Width = 400,
                Height = 200,
                Controls =
                [
                    new ControlDefinition
                    {
                        Type = controlType,
                        Text = text,
                        X = 10,
                        Y = 10,
                        Width = 100,
                        Height = 30
                    }
                ]
            }
        };

        var renderer = new SvgRenderer();
        var svg = renderer.Render(def);

        Assert.NotNull(svg);
        Assert.Contains("<svg", svg);
    }

    [Fact]
    public void Render_WithAnnotations_IncludesAnnotationElements()
    {
        var def = new ScreenDefinition
        {
            Window = new WindowDefinition
            {
                Title = "Annotated",
                Width = 400,
                Height = 200,
                Controls =
                [
                    new ControlDefinition
                    {
                        Type = "textbox",
                        Id = "input1",
                        X = 10,
                        Y = 10,
                        Width = 200,
                        Height = 25
                    }
                ]
            },
            Annotations =
            [
                new AnnotationDefinition
                {
                    Target = "input1",
                    Label = "①",
                    Description = "Test annotation"
                }
            ]
        };

        var renderer = new SvgRenderer();
        var svg = renderer.Render(def);

        Assert.Contains("①", svg);
    }

    [Fact]
    public void Render_WithConnectors_ReturnsValidSvg()
    {
        var def = new ScreenDefinition
        {
            Window = new WindowDefinition
            {
                Title = "Connectors",
                Width = 400,
                Height = 300,
                Controls =
                [
                    new ControlDefinition
                    {
                        Type = "textbox",
                        Id = "ctrl1",
                        X = 10,
                        Y = 10,
                        Width = 200,
                        Height = 25
                    },
                    new ControlDefinition
                    {
                        Type = "button",
                        Id = "ctrl2",
                        Text = "Submit",
                        X = 10,
                        Y = 80,
                        Width = 100,
                        Height = 30
                    }
                ]
            },
            Connectors =
            [
                new ConnectorDefinition
                {
                    From = "ctrl1",
                    To = "ctrl2",
                    Label = "①",
                    Description = "First step",
                    ToShape = "arrow"
                }
            ]
        };

        var renderer = new SvgRenderer();
        var svg = renderer.Render(def);

        Assert.Contains("①", svg);
    }

    [Fact]
    public void Render_WithCustomControlColors_ReturnsValidSvg()
    {
        var def = new ScreenDefinition
        {
            Window = new WindowDefinition
            {
                Title = "Custom Colors",
                Width = 400,
                Height = 200,
                Controls =
                [
                    new ControlDefinition
                    {
                        Type = "button",
                        Text = "Colored",
                        X = 10,
                        Y = 10,
                        Width = 100,
                        Height = 30,
                        Background = "#4CAF50",
                        Foreground = "#FFFFFF",
                        BorderColor = "#388E3C"
                    }
                ]
            }
        };

        var renderer = new SvgRenderer();
        var svg = renderer.Render(def);

        Assert.Contains("#4CAF50", svg);
    }

    [Fact]
    public void Render_DataGrid_ReturnsValidSvg()
    {
        var def = new ScreenDefinition
        {
            Window = new WindowDefinition
            {
                Title = "Grid Test",
                Width = 500,
                Height = 300,
                Controls =
                [
                    new ControlDefinition
                    {
                        Type = "datagrid",
                        X = 10,
                        Y = 10,
                        Width = 300,
                        Height = 150,
                        Columns =
                        [
                            new ColumnDefinition { Header = "Name", Width = 150 },
                            new ColumnDefinition { Header = "Value", Width = 150 }
                        ],
                        Rows =
                        [
                            ["Alice", "100"],
                            ["Bob", "200"]
                        ]
                    }
                ]
            }
        };

        var renderer = new SvgRenderer();
        var svg = renderer.Render(def);

        Assert.Contains("Name", svg);
        Assert.Contains("Value", svg);
        Assert.Contains("Alice", svg);
    }

    [Fact]
    public void Render_GroupWithChildren_ReturnsValidSvg()
    {
        var def = new ScreenDefinition
        {
            Window = new WindowDefinition
            {
                Title = "Group Test",
                Width = 400,
                Height = 300,
                Controls =
                [
                    new ControlDefinition
                    {
                        Type = "group",
                        Text = "Settings",
                        X = 10,
                        Y = 10,
                        Width = 300,
                        Height = 200,
                        Controls =
                        [
                            new ControlDefinition
                            {
                                Type = "label",
                                Text = "Inner Label",
                                X = 10,
                                Y = 30
                            }
                        ]
                    }
                ]
            }
        };

        var renderer = new SvgRenderer();
        var svg = renderer.Render(def);

        Assert.Contains("Settings", svg);
        Assert.Contains("Inner Label", svg);
    }
}
