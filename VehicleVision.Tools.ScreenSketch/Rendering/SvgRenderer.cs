using System.Globalization;
using System.Xml.Linq;
using VehicleVision.Tools.ScreenSketch.Models;

namespace VehicleVision.Tools.ScreenSketch.Rendering;

/// <summary>YAML 定義から SVG を生成するレンダラー</summary>
public class SvgRenderer
{
    private static readonly XNamespace Ns = "http://www.w3.org/2000/svg";
    private readonly Dictionary<string, ControlBounds> _bounds = new();

    public record ControlBounds(int X, int Y, int Width, int Height)
    {
        public int CenterY => Y + Height / 2;
        public int Right => X + Width;
    }

    // ────────────────────────────────────────────
    //  Public API
    // ────────────────────────────────────────────

    public string Render(ScreenDefinition def)
    {
        _bounds.Clear();

        var window = def.Window ?? new WindowDefinition();
        var hasAnnotations = def.Annotations is { Count: > 0 };
        var annotationMargin = hasAnnotations ? Theme.AnnotationMargin : 0;
        var pad = window.Chrome ? Theme.WindowPadding : Theme.ChromelessPadding;

        var svgWidth = window.Width + pad * 2 + annotationMargin;
        var svgHeight = window.Height + pad * 2;

        var svg = El("svg",
            new XAttribute("xmlns", Ns.NamespaceName),
            At("width", svgWidth),
            At("height", svgHeight),
            At("viewBox", $"0 0 {svgWidth} {svgHeight}"));

        svg.Add(CreateDefs());
        svg.Add(El("rect", At("width", svgWidth), At("height", svgHeight), At("fill", "#FFFFFF")));

        if (window.Chrome)
            RenderWindow(svg, window, pad, pad);
        else
            RenderChromeless(svg, window, pad, pad);

        if (def.Annotations is { Count: > 0 })
            RenderAnnotations(svg, def.Annotations, pad, pad, window.Width);

        return $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n{svg}";
    }

    // ────────────────────────────────────────────
    //  SVG ヘルパー
    // ────────────────────────────────────────────

    private static XElement El(string name, params object[] content) => new(Ns + name, content);
    private static XAttribute At(string name, object val) => new(name, val);

    private static XElement Txt(string text, int x, int y, int fontSize, string fill,
        string anchor = "start", string weight = "normal", string? fontStyle = null)
    {
        var el = El("text",
            At("x", x), At("y", y),
            At("font-size", fontSize),
            At("fill", fill),
            At("text-anchor", anchor),
            At("font-weight", weight),
            At("dominant-baseline", "central"),
            text);
        if (fontStyle != null) el.Add(At("font-style", fontStyle));
        return el;
    }

    private static XElement Rect(int x, int y, int w, int h, string fill, string stroke,
        int strokeWidth = 1, int rx = 0, string? filter = null)
    {
        var el = El("rect",
            At("x", x), At("y", y), At("width", w), At("height", h),
            At("fill", fill), At("stroke", stroke), At("stroke-width", strokeWidth));
        if (rx > 0) el.Add(At("rx", rx));
        if (filter != null) el.Add(At("filter", filter));
        return el;
    }

    private static XElement Line(int x1, int y1, int x2, int y2, string stroke,
        int strokeWidth = 1, string? dashArray = null)
    {
        var el = El("line",
            At("x1", x1), At("y1", y1), At("x2", x2), At("y2", y2),
            At("stroke", stroke), At("stroke-width", strokeWidth));
        if (dashArray != null) el.Add(At("stroke-dasharray", dashArray));
        return el;
    }

    private void Register(string? id, int x, int y, int w, int h)
    {
        if (!string.IsNullOrEmpty(id))
            _bounds[id] = new ControlBounds(x, y, w, h);
    }

    private static int EstimateTextWidth(string? text, int fontSize)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        int width = 0;
        foreach (var ch in text)
            width += ch > 0x2E80 ? fontSize : (int)(fontSize * 0.55);
        return width;
    }

    // ────────────────────────────────────────────
    //  Defs (スタイル・フィルター)
    // ────────────────────────────────────────────

    private static XElement CreateDefs()
    {
        return El("defs",
            El("filter", At("id", "windowShadow"),
                El("feDropShadow",
                    At("dx", 2), At("dy", 2),
                    At("stdDeviation", 4),
                    At("flood-opacity", 0.15))),
            El("style",
                $"text {{ font-family: {Theme.FontFamily}; }}"));
    }

    // ────────────────────────────────────────────
    //  ウィンドウ描画
    // ────────────────────────────────────────────

    /// <summary>ウィンドウ装飾なしでコンテンツ領域のみを描画する</summary>
    private void RenderChromeless(XElement svg, WindowDefinition window, int wx, int wy)
    {
        // 薄い枠線のみ
        svg.Add(Rect(wx, wy, window.Width, window.Height,
            Theme.WindowBackground, Theme.ChromelessBorder, 1, 2));

        if (window.Controls != null)
        {
            foreach (var c in window.Controls)
                RenderControl(svg, c, wx, wy, window.Width, window.Height);
        }
    }

    private void RenderWindow(XElement svg, WindowDefinition window, int wx, int wy)
    {
        // ウィンドウ背景 + 影
        svg.Add(Rect(wx, wy, window.Width, window.Height,
            Theme.WindowBackground, Theme.WindowBorder, 1, 4, "url(#windowShadow)"));

        // タイトルバー（上丸角）
        svg.Add(Rect(wx, wy, window.Width, Theme.TitleBarHeight,
            Theme.TitleBarBackground, "none", 0, 4));
        svg.Add(Rect(wx, wy + Theme.TitleBarHeight - 6, window.Width, 6,
            Theme.TitleBarBackground, "none"));

        // タイトルテキスト
        svg.Add(Txt(window.Title ?? "", wx + 10, wy + Theme.TitleBarHeight / 2,
            12, Theme.TitleBarText));

        // ウィンドウボタン
        RenderWindowButtons(svg, wx, wy, window.Width);

        // コンテンツ領域
        var contentX = wx;
        var contentY = wy + Theme.TitleBarHeight;
        var contentW = window.Width;
        var contentH = window.Height - Theme.TitleBarHeight;

        if (window.Controls != null)
        {
            foreach (var c in window.Controls)
                RenderControl(svg, c, contentX, contentY, contentW, contentH);
        }
    }

    private static void RenderWindowButtons(XElement svg, int wx, int wy, int windowWidth)
    {
        var btnY = wy + Theme.TitleBarHeight / 2;

        // Close (×)
        var cx = wx + windowWidth - 22;
        svg.Add(Line(cx, btnY - 4, cx + 8, btnY + 4, Theme.TitleBarText));
        svg.Add(Line(cx, btnY + 4, cx + 8, btnY - 4, Theme.TitleBarText));

        // Maximize (□)
        var mx = cx - 25;
        svg.Add(Rect(mx, btnY - 4, 8, 8, "none", Theme.TitleBarText));

        // Minimize (─)
        var minX = mx - 25;
        svg.Add(Line(minX, btnY, minX + 8, btnY, Theme.TitleBarText));
    }

    // ────────────────────────────────────────────
    //  コントロール分岐
    // ────────────────────────────────────────────

    private void RenderControl(XElement svg, ControlDefinition c,
        int offsetX, int offsetY, int containerW, int containerH)
    {
        ApplyDefaults(c, containerW, containerH);

        var ax = offsetX + c.X;
        var ay = offsetY + c.Y;
        Register(c.Id, ax, ay, c.Width, c.Height);

        switch (c.Type.ToLowerInvariant())
        {
            case "button": RenderButton(svg, ax, ay, c.Width, c.Height, c.Text); break;
            case "textbox": RenderTextBox(svg, ax, ay, c.Width, c.Height, c.Text, c.Placeholder); break;
            case "label": RenderStaticLabel(svg, ax, ay, c.Height, c.Text); break;
            case "combobox": RenderComboBox(svg, ax, ay, c.Width, c.Height, c.Text, c.Items); break;
            case "checkbox": RenderCheckBox(svg, ax, ay, c.Text, c.Checked); break;
            case "radiobutton": RenderRadioButton(svg, ax, ay, c.Text, c.Selected); break;
            case "group": RenderGroupBox(svg, c, ax, ay); break;
            case "datagrid": RenderDataGrid(svg, c, ax, ay); break;
            case "menubar": RenderMenuBar(svg, ax, ay, c.Width, c.Height, c.Items); break;
            case "statusbar": RenderStatusBar(svg, ax, ay, c.Width, c.Height, c.Items); break;
            case "tabcontrol": RenderTabControl(svg, c, ax, ay); break;
            case "listbox": RenderListBox(svg, ax, ay, c.Width, c.Height, c.Items, c.SelectedIndex); break;
            case "panel": RenderPanel(svg, c, ax, ay); break;
            case "image": RenderImagePlaceholder(svg, ax, ay, c.Width, c.Height, c.Text); break;
            case "progressbar": RenderProgressBar(svg, ax, ay, c.Width, c.Height, c.Value); break;
            case "numericupdown": RenderNumericUpDown(svg, ax, ay, c.Width, c.Height, c.Value, c.Minimum, c.Maximum); break;
            case "datetimepicker": RenderDateTimePicker(svg, ax, ay, c.Width, c.Height, c.DateText, c.Format); break;
            case "treeview": RenderTreeView(svg, ax, ay, c.Width, c.Height, c.Nodes); break;
            case "toolbar": RenderToolbar(svg, ax, ay, c.Width, c.Height, c.Items); break;
            case "linklabel": RenderLinkLabel(svg, ax, ay, c.Height, c.Text); break;
            case "textarea": RenderTextArea(svg, ax, ay, c.Width, c.Height, c.Text, c.Placeholder, c.ReadOnly); break;
        }
    }

    private static void ApplyDefaults(ControlDefinition c, int containerW, int containerH)
    {
        switch (c.Type.ToLowerInvariant())
        {
            case "menubar":
                if (c.Width == 0) c.Width = containerW;
                if (c.Height == 0) c.Height = Theme.MenuBarHeight;
                break;
            case "statusbar":
                if (c.Width == 0) c.Width = containerW;
                if (c.Height == 0) c.Height = Theme.StatusBarHeight;
                if (c.X == 0 && c.Y == 0) c.Y = containerH - c.Height;
                break;
            case "button":
                if (c.Width == 0) c.Width = 80;
                if (c.Height == 0) c.Height = 26;
                break;
            case "textbox":
            case "combobox":
                if (c.Width == 0) c.Width = 150;
                if (c.Height == 0) c.Height = 24;
                break;
            case "checkbox":
            case "radiobutton":
                if (c.Width == 0) c.Width = EstimateTextWidth(c.Text, 12) + 22;
                if (c.Height == 0) c.Height = 20;
                break;
            case "label":
                if (c.Height == 0) c.Height = 20;
                break;
            case "progressbar":
                if (c.Width == 0) c.Width = 200;
                if (c.Height == 0) c.Height = 20;
                break;
            case "numericupdown":
                if (c.Width == 0) c.Width = 100;
                if (c.Height == 0) c.Height = 24;
                break;
            case "datetimepicker":
                if (c.Width == 0) c.Width = 180;
                if (c.Height == 0) c.Height = 24;
                break;
            case "toolbar":
                if (c.Width == 0) c.Width = containerW;
                if (c.Height == 0) c.Height = 28;
                break;
            case "linklabel":
                if (c.Height == 0) c.Height = 20;
                break;
            case "textarea":
                if (c.Width == 0) c.Width = 200;
                if (c.Height == 0) c.Height = 80;
                break;
        }
    }

    // ────────────────────────────────────────────
    //  コントロール描画
    // ────────────────────────────────────────────

    private static void RenderButton(XElement svg, int x, int y, int w, int h, string? text)
    {
        svg.Add(Rect(x, y, w, h, Theme.ButtonBackground, Theme.ButtonBorder, 1, 3));
        if (!string.IsNullOrEmpty(text))
            svg.Add(Txt(text, x + w / 2, y + h / 2, 12, Theme.ButtonText, "middle"));
    }

    private static void RenderTextBox(XElement svg, int x, int y, int w, int h,
        string? text, string? placeholder)
    {
        svg.Add(Rect(x, y, w, h, Theme.TextBoxBackground, Theme.TextBoxBorder, 1, 2));

        var display = !string.IsNullOrEmpty(text) ? text : placeholder;
        var fill = !string.IsNullOrEmpty(text) ? Theme.TextBoxText : Theme.TextBoxPlaceholder;
        var style = string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(placeholder) ? "italic" : null;

        if (!string.IsNullOrEmpty(display))
            svg.Add(Txt(display, x + 4, y + h / 2, 12, fill, fontStyle: style));
    }

    private static void RenderStaticLabel(XElement svg, int x, int y, int h, string? text)
    {
        if (!string.IsNullOrEmpty(text))
            svg.Add(Txt(text, x, y + h / 2, 12, Theme.LabelText));
    }

    private static void RenderComboBox(XElement svg, int x, int y, int w, int h,
        string? text, List<string>? items)
    {
        svg.Add(Rect(x, y, w, h, Theme.TextBoxBackground, Theme.TextBoxBorder, 1, 2));

        var display = text ?? items?.FirstOrDefault() ?? "";
        if (!string.IsNullOrEmpty(display))
            svg.Add(Txt(display, x + 4, y + h / 2, 12, Theme.TextBoxText));

        // ドロップダウンボタン
        svg.Add(Rect(x + w - 20, y, 20, h, Theme.ButtonBackground, Theme.TextBoxBorder, 1, 2));
        var am = x + w - 10;
        var arrowY = y + h / 2 - 3;
        svg.Add(El("polygon",
            At("points", $"{x + w - 15},{arrowY} {x + w - 5},{arrowY} {am},{arrowY + 6}"),
            At("fill", "#666666")));
    }

    private static void RenderCheckBox(XElement svg, int x, int y, string? text, bool isChecked)
    {
        var boxSize = 14;
        var boxY = y + 3;
        svg.Add(Rect(x, boxY, boxSize, boxSize, Theme.TextBoxBackground, "#666666", 1, 2));

        if (isChecked)
        {
            svg.Add(El("polyline",
                At("points", $"{x + 3},{boxY + 7} {x + 6},{boxY + 10} {x + 11},{boxY + 3}"),
                At("fill", "none"),
                At("stroke", Theme.TitleBarBackground),
                At("stroke-width", 2)));
        }

        if (!string.IsNullOrEmpty(text))
            svg.Add(Txt(text, x + boxSize + 6, y + 10, 12, Theme.LabelText));
    }

    private static void RenderRadioButton(XElement svg, int x, int y, string? text, bool selected)
    {
        var r = 7;
        var cx = x + r;
        var cy = y + 10;
        svg.Add(El("circle", At("cx", cx), At("cy", cy), At("r", r),
            At("fill", Theme.TextBoxBackground), At("stroke", "#666666"), At("stroke-width", 1)));

        if (selected)
            svg.Add(El("circle", At("cx", cx), At("cy", cy), At("r", 4),
                At("fill", Theme.TitleBarBackground)));

        if (!string.IsNullOrEmpty(text))
            svg.Add(Txt(text, x + r * 2 + 6, y + 10, 12, Theme.LabelText));
    }

    private void RenderGroupBox(XElement svg, ControlDefinition c, int ax, int ay)
    {
        var w = c.Width;
        var h = c.Height;

        // 枠線（ラベル分下げる）
        svg.Add(Rect(ax, ay + 8, w, h - 8, "none", Theme.GroupBorder, 1, 3));

        // ラベル背景 + テキスト
        var labelW = EstimateTextWidth(c.Text, 11) + 12;
        svg.Add(Rect(ax + 8, ay, labelW, 16, Theme.WindowBackground, "none"));
        if (!string.IsNullOrEmpty(c.Text))
            svg.Add(Txt(c.Text, ax + 14, ay + 8, 11, Theme.GroupText));

        // 子コントロール
        if (c.Controls != null)
        {
            foreach (var child in c.Controls)
                RenderControl(svg, child, ax, ay + 16, w, h - 16);
        }
    }

    private static void RenderDataGrid(XElement svg, ControlDefinition c, int ax, int ay)
    {
        var w = c.Width;
        var h = c.Height;
        var headerH = 26;
        var rowH = 24;

        svg.Add(Rect(ax, ay, w, h, Theme.GridBackground, Theme.GridBorder));
        svg.Add(Rect(ax, ay, w, headerH, Theme.GridHeaderBackground, Theme.GridBorder));

        if (c.Columns != null)
        {
            var colX = ax;
            for (var i = 0; i < c.Columns.Count; i++)
            {
                var col = c.Columns[i];
                var colW = col.Width > 0 ? col.Width : w / c.Columns.Count;

                svg.Add(Txt(col.Header, colX + 8, ay + headerH / 2, 11,
                    Theme.GridHeaderText, weight: "bold"));

                if (i > 0)
                    svg.Add(Line(colX, ay, colX, ay + h, Theme.GridBorder));

                colX += colW;
            }
        }

        svg.Add(Line(ax, ay + headerH, ax + w, ay + headerH, Theme.GridBorder));

        if (c.Rows != null && c.Columns != null)
        {
            for (var ri = 0; ri < c.Rows.Count; ri++)
            {
                var row = c.Rows[ri];
                var rowY = ay + headerH + ri * rowH;
                if (rowY + rowH > ay + h) break;

                if (ri % 2 == 1)
                    svg.Add(Rect(ax + 1, rowY, w - 2, rowH, Theme.GridRowAlternate, "none"));

                svg.Add(Line(ax, rowY, ax + w, rowY, Theme.GridBorder));

                var colX = ax;
                for (var ci = 0; ci < Math.Min(row.Count, c.Columns.Count); ci++)
                {
                    var colW = c.Columns[ci].Width > 0 ? c.Columns[ci].Width : w / c.Columns.Count;
                    svg.Add(Txt(row[ci], colX + 8, rowY + rowH / 2, 11, Theme.GridCellText));
                    colX += colW;
                }
            }
        }
    }

    private static void RenderMenuBar(XElement svg, int x, int y, int w, int h, List<string>? items)
    {
        svg.Add(Rect(x, y, w, h, Theme.MenuBackground, "none"));
        svg.Add(Line(x, y + h, x + w, y + h, Theme.MenuBorder));

        if (items != null)
        {
            var itemX = x + 8;
            foreach (var item in items)
            {
                svg.Add(Txt(item, itemX, y + h / 2, 12, Theme.MenuText));
                itemX += EstimateTextWidth(item, 12) + 16;
            }
        }
    }

    private static void RenderStatusBar(XElement svg, int x, int y, int w, int h, List<string>? items)
    {
        svg.Add(Rect(x, y, w, h, Theme.StatusBarBackground, "none"));

        if (items != null)
        {
            var itemX = x + 10;
            for (var i = 0; i < items.Count; i++)
            {
                svg.Add(Txt(items[i], itemX, y + h / 2, 11, Theme.StatusBarText));
                itemX += EstimateTextWidth(items[i], 11) + 20;

                if (i < items.Count - 1)
                    svg.Add(Line(itemX - 10, y + 3, itemX - 10, y + h - 3, "#FFFFFF"));
            }
        }
    }

    private void RenderTabControl(XElement svg, ControlDefinition c, int ax, int ay)
    {
        var tabH = 26;
        var w = c.Width;
        var h = c.Height;

        svg.Add(Rect(ax, ay + tabH, w, h - tabH, Theme.TextBoxBackground, Theme.GridBorder));

        if (c.Tabs != null)
        {
            var tabX = ax;
            for (var i = 0; i < c.Tabs.Count; i++)
            {
                var tab = c.Tabs[i];
                var tabW = EstimateTextWidth(tab.Text, 12) + 24;
                var isActive = i == c.ActiveTab;
                var tabFill = isActive ? Theme.TextBoxBackground : Theme.GridHeaderBackground;

                svg.Add(Rect(tabX, ay, tabW, tabH + 1, tabFill, Theme.GridBorder, 1, 3));
                if (isActive)
                    svg.Add(Rect(tabX + 1, ay + tabH - 1, tabW - 2, 3, Theme.TextBoxBackground, "none"));

                svg.Add(Txt(tab.Text, tabX + 12, ay + tabH / 2, 12,
                    isActive ? Theme.LabelText : Theme.GroupText));

                tabX += tabW + 2;
            }
        }

        if (c.Tabs != null && c.ActiveTab >= 0 && c.ActiveTab < c.Tabs.Count)
        {
            var activeTab = c.Tabs[c.ActiveTab];
            if (activeTab.Controls != null)
            {
                foreach (var child in activeTab.Controls)
                    RenderControl(svg, child, ax, ay + tabH, w, h - tabH);
            }
        }
    }

    private static void RenderListBox(XElement svg, int x, int y, int w, int h,
        List<string>? items, int selectedIndex)
    {
        svg.Add(Rect(x, y, w, h, Theme.TextBoxBackground, Theme.TextBoxBorder));

        if (items != null)
        {
            var itemH = 22;
            for (var i = 0; i < items.Count; i++)
            {
                var itemY = y + i * itemH;
                if (itemY + itemH > y + h) break;

                if (i == selectedIndex)
                    svg.Add(Rect(x + 1, itemY, w - 2, itemH, "#0078D4", "none"));

                svg.Add(Txt(items[i], x + 6, itemY + itemH / 2, 12,
                    i == selectedIndex ? "#FFFFFF" : Theme.LabelText));
            }
        }
    }

    private void RenderPanel(XElement svg, ControlDefinition c, int ax, int ay)
    {
        svg.Add(Rect(ax, ay, c.Width, c.Height, "none", Theme.GridBorder, 1, 2));
        if (c.Controls != null)
        {
            foreach (var child in c.Controls)
                RenderControl(svg, child, ax, ay, c.Width, c.Height);
        }
    }

    private static void RenderImagePlaceholder(XElement svg, int x, int y, int w, int h, string? text)
    {
        svg.Add(Rect(x, y, w, h, "#F5F5F5", Theme.GridBorder, 1, 2));

        var cx = x + w / 2;
        var cy = y + h / 2 - 10;
        svg.Add(El("polygon",
            At("points", $"{cx - 20},{cy + 15} {cx - 5},{cy - 5} {cx + 5},{cy + 5} {cx + 20},{cy + 15}"),
            At("fill", "#C0C0C0")));
        svg.Add(El("circle", At("cx", cx + 10), At("cy", cy - 8), At("r", 5), At("fill", "#C0C0C0")));

        var label = !string.IsNullOrEmpty(text) ? text : "画像";
        svg.Add(Txt(label, cx, cy + 30, 11, "#999999", "middle"));
    }

    private static void RenderProgressBar(XElement svg, int x, int y, int w, int h, int value)
    {
        var clamped = Math.Clamp(value, 0, 100);
        svg.Add(Rect(x, y, w, h, "#E0E0E0", Theme.GridBorder, 1, 3));

        var fillW = (int)((w - 2) * clamped / 100.0);
        if (fillW > 0)
            svg.Add(Rect(x + 1, y + 1, fillW, h - 2, Theme.TitleBarBackground, "none", 0, 2));

        svg.Add(Txt($"{clamped}%", x + w / 2, y + h / 2, 10, Theme.LabelText, "middle", "bold"));
    }

    private static void RenderNumericUpDown(XElement svg, int x, int y, int w, int h,
        int value, int minimum, int maximum)
    {
        var clamped = Math.Clamp(value, minimum, maximum);
        var btnW = 18;

        // テキストエリア
        svg.Add(Rect(x, y, w, h, Theme.TextBoxBackground, Theme.TextBoxBorder, 1, 2));
        svg.Add(Txt(clamped.ToString(CultureInfo.InvariantCulture), x + 4, y + h / 2, 12, Theme.TextBoxText));

        // 上ボタン
        svg.Add(Rect(x + w - btnW, y, btnW, h / 2, Theme.ButtonBackground, Theme.TextBoxBorder, 1, 0));
        var upX = x + w - btnW / 2;
        var upY = y + h / 4;
        svg.Add(El("polygon",
            At("points", $"{upX - 4},{upY + 2} {upX + 4},{upY + 2} {upX},{upY - 3}"),
            At("fill", "#666666")));

        // 下ボタン
        svg.Add(Rect(x + w - btnW, y + h / 2, btnW, h / 2, Theme.ButtonBackground, Theme.TextBoxBorder, 1, 0));
        var dnY = y + h * 3 / 4;
        svg.Add(El("polygon",
            At("points", $"{upX - 4},{dnY - 2} {upX + 4},{dnY - 2} {upX},{dnY + 3}"),
            At("fill", "#666666")));
    }

    private static void RenderDateTimePicker(XElement svg, int x, int y, int w, int h,
        string? dateText, string? format)
    {
        svg.Add(Rect(x, y, w, h, Theme.TextBoxBackground, Theme.TextBoxBorder, 1, 2));

        var display = dateText;
        if (string.IsNullOrEmpty(display))
        {
            display = (format?.ToLowerInvariant()) switch
            {
                "time" => "00:00:00",
                "long" => "2026年1月1日",
                _ => "2026/01/01"
            };
        }

        svg.Add(Txt(display, x + 4, y + h / 2, 12, Theme.TextBoxText));

        // カレンダーボタン
        var btnW = 20;
        svg.Add(Rect(x + w - btnW, y, btnW, h, Theme.ButtonBackground, Theme.TextBoxBorder, 1, 2));

        // カレンダーアイコン（簡略化）
        var ix = x + w - btnW / 2;
        var iy = y + h / 2;
        svg.Add(Rect(ix - 5, iy - 5, 10, 10, "none", "#666666", 1, 1));
        svg.Add(Line(ix - 5, iy - 2, ix + 5, iy - 2, "#666666"));
        svg.Add(El("circle", At("cx", ix), At("cy", iy + 2), At("r", 1), At("fill", "#666666")));
    }

    private static void RenderTreeView(XElement svg, int x, int y, int w, int h,
        List<TreeNodeDefinition>? nodes)
    {
        svg.Add(Rect(x, y, w, h, Theme.TreeViewBackground, Theme.TextBoxBorder));

        if (nodes != null)
        {
            var currentY = y + 4;
            foreach (var node in nodes)
                currentY = RenderTreeNode(svg, node, x + 6, currentY, 0, y + h);
        }
    }

    private static int RenderTreeNode(XElement svg, TreeNodeDefinition node, int x, int y,
        int depth, int maxY)
    {
        var nodeH = 20;
        var indent = depth * 16;
        var nx = x + indent;

        if (y + nodeH > maxY) return y;

        // 展開/折りたたみアイコン
        if (node.Children is { Count: > 0 })
        {
            if (node.Expanded)
            {
                // ▼
                svg.Add(El("polygon",
                    At("points", $"{nx},{y + 5} {nx + 8},{y + 5} {nx + 4},{y + 12}"),
                    At("fill", Theme.TreeViewExpander)));
            }
            else
            {
                // ▶
                svg.Add(El("polygon",
                    At("points", $"{nx},{y + 4} {nx},{y + 14} {nx + 7},{y + 9}"),
                    At("fill", Theme.TreeViewExpander)));
            }
        }

        // ノードテキスト
        svg.Add(Txt(node.Text, nx + 14, y + nodeH / 2, 12, Theme.TreeViewText));

        var nextY = y + nodeH;

        if (node.Expanded && node.Children != null)
        {
            foreach (var child in node.Children)
            {
                nextY = RenderTreeNode(svg, child, x, nextY, depth + 1, maxY);
                if (nextY + nodeH > maxY) break;
            }
        }

        return nextY;
    }

    private static void RenderToolbar(XElement svg, int x, int y, int w, int h, List<string>? items)
    {
        svg.Add(Rect(x, y, w, h, Theme.ToolbarBackground, "none"));
        svg.Add(Line(x, y + h, x + w, y + h, Theme.ToolbarBorder));

        if (items != null)
        {
            var itemX = x + 4;
            foreach (var item in items)
            {
                if (item == "|" || item == "-")
                {
                    // セパレータ
                    svg.Add(Line(itemX + 2, y + 4, itemX + 2, y + h - 4, Theme.ToolbarSeparator));
                    itemX += 8;
                }
                else
                {
                    var btnW = EstimateTextWidth(item, 11) + 12;
                    svg.Add(Rect(itemX, y + 2, btnW, h - 4, "none", "none", 0, 3));
                    svg.Add(Txt(item, itemX + 6, y + h / 2, 11, Theme.ToolbarText));
                    itemX += btnW + 2;
                }
            }
        }
    }

    private static void RenderLinkLabel(XElement svg, int x, int y, int h, string? text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            var textW = EstimateTextWidth(text, 12);
            svg.Add(Txt(text, x, y + h / 2, 12, Theme.LinkLabelText));
            svg.Add(Line(x, y + h / 2 + 8, x + textW, y + h / 2 + 8, Theme.LinkLabelUnderline));
        }
    }

    private static void RenderTextArea(XElement svg, int x, int y, int w, int h,
        string? text, string? placeholder, bool readOnly)
    {
        var bgColor = readOnly ? "#F5F5F5" : Theme.TextBoxBackground;
        svg.Add(Rect(x, y, w, h, bgColor, Theme.TextBoxBorder, 1, 2));

        // スクロールバー
        var scrollW = 14;
        svg.Add(Rect(x + w - scrollW, y + 1, scrollW - 1, h - 2, "#F0F0F0", Theme.GridBorder, 1, 0));
        // スクロールサム
        svg.Add(Rect(x + w - scrollW + 2, y + 4, scrollW - 5, 20, "#C0C0C0", "none", 0, 3));

        var display = !string.IsNullOrEmpty(text) ? text : placeholder;
        var fill = !string.IsNullOrEmpty(text) ? Theme.TextBoxText : Theme.TextBoxPlaceholder;
        var style = string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(placeholder) ? "italic" : null;

        if (!string.IsNullOrEmpty(display))
        {
            // 複数行表示（改行で分割）
            var lines = display.Split('\n');
            var lineH = 18;
            for (var i = 0; i < lines.Length; i++)
            {
                var ly = y + 12 + i * lineH;
                if (ly > y + h - 6) break;
                svg.Add(Txt(lines[i], x + 4, ly, 12, fill, fontStyle: style));
            }
        }
    }

    // ────────────────────────────────────────────
    //  アノテーション描画
    // ────────────────────────────────────────────

    private void RenderAnnotations(XElement svg, List<AnnotationDefinition> annotations,
        int windowX, int windowY, int windowWidth)
    {
        var annotationX = windowX + windowWidth + 30;

        var sorted = annotations
            .Where(a => _bounds.ContainsKey(a.Target))
            .OrderBy(a => _bounds[a.Target].CenterY)
            .ToList();

        // 最小間隔を保って配置
        var minSpacing = Theme.AnnotationRadius * 2 + 8;
        var positions = new List<(AnnotationDefinition Ann, int CircleY)>();
        var lastY = windowY;

        foreach (var ann in sorted)
        {
            var target = _bounds[ann.Target];
            var desiredY = target.CenterY;
            var circleY = Math.Max(desiredY, lastY + minSpacing);
            positions.Add((ann, circleY));
            lastY = circleY;
        }

        foreach (var (ann, circleY) in positions)
        {
            var target = _bounds[ann.Target];
            var circleX = annotationX;

            // リーダー線
            var lineStartX = target.Right + 2;
            var lineStartY = target.CenterY;
            var lineEndX = circleX - Theme.AnnotationRadius - 2;

            svg.Add(Line(lineStartX, lineStartY, lineEndX, circleY,
                Theme.AnnotationLine, 1, "4,3"));

            // コントロール接続点
            svg.Add(El("circle",
                At("cx", lineStartX), At("cy", lineStartY), At("r", 3),
                At("fill", Theme.AnnotationCircle)));

            // ラベル円
            var radius = Math.Max(Theme.AnnotationRadius, EstimateTextWidth(ann.Label, 13) / 2 + 4);
            svg.Add(El("circle",
                At("cx", circleX), At("cy", circleY), At("r", radius),
                At("fill", Theme.AnnotationCircle)));

            svg.Add(Txt(ann.Label, circleX, circleY, 13,
                Theme.AnnotationText, "middle", "bold"));
        }
    }
}
