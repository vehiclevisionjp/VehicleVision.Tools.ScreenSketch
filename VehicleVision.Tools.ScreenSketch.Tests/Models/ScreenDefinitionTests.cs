using VehicleVision.Tools.ScreenSketch.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace VehicleVision.Tools.ScreenSketch.Tests.Models;

public class ScreenDefinitionTests
{
    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    [Fact]
    public void Deserialize_MinimalYaml_ReturnsValidDefinition()
    {
        const string yaml = """
                            screen:
                              title: テスト画面

                            window:
                              title: サンプル
                              width: 400
                              height: 200
                              controls:
                                - type: label
                                  text: "Hello, World!"
                                  x: 20
                                  y: 20
                            """;

        var def = Deserializer.Deserialize<ScreenDefinition>(yaml);

        Assert.NotNull(def.Screen);
        Assert.Equal("テスト画面", def.Screen.Title);
        Assert.NotNull(def.Window);
        Assert.Equal("サンプル", def.Window.Title);
        Assert.Equal(400, def.Window.Width);
        Assert.Equal(200, def.Window.Height);
        Assert.NotNull(def.Window.Controls);
        Assert.Single(def.Window.Controls);
        Assert.Equal("label", def.Window.Controls[0].Type);
        Assert.Equal("Hello, World!", def.Window.Controls[0].Text);
    }

    [Fact]
    public void Deserialize_WithAnnotations_ParsesCorrectly()
    {
        const string yaml = """
                            screen:
                              title: Annotated

                            window:
                              title: Test
                              width: 400
                              height: 200
                              controls:
                                - type: textbox
                                  id: input1
                                  x: 10
                                  y: 10
                                  width: 200

                            annotations:
                              - target: input1
                                label: "①"
                                description: テスト説明
                                lineColor: "#FF0000"
                                lineStyle: dotted
                                labelBackground: "#0000FF"
                                labelColor: "#FFFFFF"
                            """;

        var def = Deserializer.Deserialize<ScreenDefinition>(yaml);

        Assert.NotNull(def.Annotations);
        Assert.Single(def.Annotations);
        var ann = def.Annotations[0];
        Assert.Equal("input1", ann.Target);
        Assert.Equal("①", ann.Label);
        Assert.Equal("テスト説明", ann.Description);
        Assert.Equal("#FF0000", ann.LineColor);
        Assert.Equal("dotted", ann.LineStyle);
        Assert.Equal("#0000FF", ann.LabelBackground);
        Assert.Equal("#FFFFFF", ann.LabelColor);
    }

    [Fact]
    public void Deserialize_WithConnectors_ParsesCorrectly()
    {
        const string yaml = """
                            screen:
                              title: Connectors

                            window:
                              title: Test
                              width: 400
                              height: 300
                              controls:
                                - type: textbox
                                  id: ctrl1
                                  x: 10
                                  y: 10
                                  width: 200
                                - type: button
                                  id: ctrl2
                                  x: 10
                                  y: 80
                                  text: Submit

                            connectors:
                              - from: ctrl1
                                to: ctrl2
                                label: "①"
                                description: 入力後にボタンを押す
                                lineColor: "#4CAF50"
                                lineStyle: dashed
                                fromShape: circle
                                toShape: arrow
                                fromAnchor: bottom
                                toAnchor: top
                                lineType: curve
                            """;

        var def = Deserializer.Deserialize<ScreenDefinition>(yaml);

        Assert.NotNull(def.Connectors);
        Assert.Single(def.Connectors);
        var conn = def.Connectors[0];
        Assert.Equal("ctrl1", conn.From);
        Assert.Equal("ctrl2", conn.To);
        Assert.Equal("①", conn.Label);
        Assert.Equal("#4CAF50", conn.LineColor);
        Assert.Equal("dashed", conn.LineStyle);
        Assert.Equal("circle", conn.FromShape);
        Assert.Equal("arrow", conn.ToShape);
        Assert.Equal("bottom", conn.FromAnchor);
        Assert.Equal("top", conn.ToAnchor);
        Assert.Equal("curve", conn.LineType);
    }

    [Fact]
    public void Deserialize_WithTheme_ParsesCorrectly()
    {
        const string yaml = """
                            screen:
                              title: Custom Theme
                              theme: custom
                              customTheme:
                                windowBackground: "#AABBCC"
                                titleBarBackground: "#112233"

                            window:
                              title: Test
                              width: 400
                              height: 200
                            """;

        var def = Deserializer.Deserialize<ScreenDefinition>(yaml);

        Assert.NotNull(def.Screen);
        Assert.Equal("custom", def.Screen.Theme);
        Assert.NotNull(def.Screen.CustomTheme);
        Assert.Equal("#AABBCC", def.Screen.CustomTheme["windowBackground"]);
        Assert.Equal("#112233", def.Screen.CustomTheme["titleBarBackground"]);
    }

    [Fact]
    public void Deserialize_DataGrid_ParsesColumnsAndRows()
    {
        const string yaml = """
                            screen:
                              title: Grid

                            window:
                              title: Test
                              width: 500
                              height: 300
                              controls:
                                - type: datagrid
                                  x: 10
                                  y: 10
                                  width: 300
                                  height: 150
                                  columns:
                                    - header: Name
                                      width: 150
                                    - header: Age
                                      width: 100
                                  rows:
                                    - [Alice, "30"]
                                    - [Bob, "25"]
                            """;

        var def = Deserializer.Deserialize<ScreenDefinition>(yaml);

        Assert.NotNull(def.Window?.Controls);
        var grid = def.Window.Controls[0];
        Assert.Equal("datagrid", grid.Type);
        Assert.NotNull(grid.Columns);
        Assert.Equal(2, grid.Columns.Count);
        Assert.Equal("Name", grid.Columns[0].Header);
        Assert.NotNull(grid.Rows);
        Assert.Equal(2, grid.Rows.Count);
        Assert.Equal("Alice", grid.Rows[0][0]);
    }

    [Fact]
    public void Deserialize_TabControl_ParsesTabs()
    {
        const string yaml = """
                            screen:
                              title: Tabs

                            window:
                              title: Test
                              width: 500
                              height: 300
                              controls:
                                - type: tabcontrol
                                  x: 10
                                  y: 10
                                  width: 400
                                  height: 250
                                  activeTab: 1
                                  tabs:
                                    - text: Tab1
                                      controls:
                                        - type: label
                                          text: Content1
                                          x: 10
                                          y: 10
                                    - text: Tab2
                                      controls:
                                        - type: label
                                          text: Content2
                                          x: 10
                                          y: 10
                            """;

        var def = Deserializer.Deserialize<ScreenDefinition>(yaml);

        var tabControl = def.Window!.Controls![0];
        Assert.Equal("tabcontrol", tabControl.Type);
        Assert.Equal(1, tabControl.ActiveTab);
        Assert.NotNull(tabControl.Tabs);
        Assert.Equal(2, tabControl.Tabs.Count);
        Assert.Equal("Tab1", tabControl.Tabs[0].Text);
        Assert.NotNull(tabControl.Tabs[0].Controls);
    }

    [Fact]
    public void Deserialize_TreeView_ParsesNodes()
    {
        const string yaml = """
                            screen:
                              title: Tree

                            window:
                              title: Test
                              width: 400
                              height: 300
                              controls:
                                - type: treeview
                                  x: 10
                                  y: 10
                                  width: 200
                                  height: 200
                                  nodes:
                                    - text: Root
                                      expanded: true
                                      children:
                                        - text: Child1
                                        - text: Child2
                                          children:
                                            - text: Grandchild
                            """;

        var def = Deserializer.Deserialize<ScreenDefinition>(yaml);

        var tree = def.Window!.Controls![0];
        Assert.Equal("treeview", tree.Type);
        Assert.NotNull(tree.Nodes);
        Assert.Single(tree.Nodes);
        Assert.Equal("Root", tree.Nodes[0].Text);
        Assert.True(tree.Nodes[0].Expanded);
        Assert.NotNull(tree.Nodes[0].Children);
        Assert.Equal(2, tree.Nodes[0].Children!.Count);
    }

    [Fact]
    public void Deserialize_ControlCustomColors_ParsesCorrectly()
    {
        const string yaml = """
                            screen:
                              title: Colors

                            window:
                              title: Test
                              width: 400
                              height: 200
                              controls:
                                - type: button
                                  text: Colored
                                  x: 10
                                  y: 10
                                  background: "#4CAF50"
                                  foreground: "#FFFFFF"
                                  borderColor: "#388E3C"
                            """;

        var def = Deserializer.Deserialize<ScreenDefinition>(yaml);

        var button = def.Window!.Controls![0];
        Assert.Equal("#4CAF50", button.Background);
        Assert.Equal("#FFFFFF", button.Foreground);
        Assert.Equal("#388E3C", button.BorderColor);
    }

    [Fact]
    public void Deserialize_ChromelessWindow_ParsesCorrectly()
    {
        const string yaml = """
                            screen:
                              title: Chromeless

                            window:
                              title: Test
                              width: 400
                              height: 200
                              chrome: false
                            """;

        var def = Deserializer.Deserialize<ScreenDefinition>(yaml);

        Assert.False(def.Window!.Chrome);
    }

    [Fact]
    public void WindowDefinition_Defaults_AreCorrect()
    {
        var window = new WindowDefinition();
        Assert.Equal("", window.Title);
        Assert.Equal(800, window.Width);
        Assert.Equal(600, window.Height);
        Assert.True(window.Chrome);
    }

    [Fact]
    public void ControlDefinition_Defaults_AreCorrect()
    {
        var control = new ControlDefinition();
        Assert.Equal("", control.Type);
        Assert.Null(control.Id);
        Assert.Equal(0, control.X);
        Assert.Equal(0, control.Y);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.False(control.Checked);
        Assert.False(control.Selected);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(0, control.Value);
        Assert.Equal(100, control.Maximum);
        Assert.False(control.ReadOnly);
    }
}
