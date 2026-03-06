using System.Globalization;
using System.Xml.Linq;
using VehicleVision.Tools.ScreenSketch.Models;

namespace VehicleVision.Tools.ScreenSketch.Rendering;

/// <summary>YAML 定義から SVG を生成するレンダラー</summary>
public class SvgRenderer
{
    private static readonly XNamespace Ns = "http://www.w3.org/2000/svg";
    private readonly Dictionary<string, ControlBounds> _bounds = new();
    private readonly ThemeColors _colors;

    public SvgRenderer() : this(new ThemeColors()) { }

    public SvgRenderer(ThemeColors colors)
    {
        _colors = colors;
    }

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
        svg.Add(El("rect", At("width", svgWidth), At("height", svgHeight), At("fill", _colors.CanvasBackground)));

        if (window.Chrome)
            RenderWindow(svg, window, pad, pad);
        else
            RenderChromeless(svg, window, pad, pad);

        if (def.Connectors is { Count: > 0 })
            RenderConnectors(svg, def.Connectors);

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

    /// <summary>ユーザー指定の色値を解決する。名前付き色（WinForms KnownColor）をサポート</summary>
    private static string ResolveColor(string? userColor, string fallback)
        => ColorResolver.Resolve(userColor, fallback);

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
            _colors.WindowBackground, _colors.ChromelessBorder, 1, 2));

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
            _colors.WindowBackground, _colors.WindowBorder, 1, 4, "url(#windowShadow)"));

        // タイトルバー（上丸角）
        svg.Add(Rect(wx, wy, window.Width, Theme.TitleBarHeight,
            _colors.TitleBarBackground, "none", 0, 4));
        svg.Add(Rect(wx, wy + Theme.TitleBarHeight - 6, window.Width, 6,
            _colors.TitleBarBackground, "none"));

        // タイトルテキスト
        svg.Add(Txt(window.Title ?? "", wx + 10, wy + Theme.TitleBarHeight / 2,
            12, _colors.TitleBarText));

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

    private void RenderWindowButtons(XElement svg, int wx, int wy, int windowWidth)
    {
        var btnY = wy + Theme.TitleBarHeight / 2;

        // Close (×)
        var cx = wx + windowWidth - 22;
        svg.Add(Line(cx, btnY - 4, cx + 8, btnY + 4, _colors.TitleBarText));
        svg.Add(Line(cx, btnY + 4, cx + 8, btnY - 4, _colors.TitleBarText));

        // Maximize (□)
        var mx = cx - 25;
        svg.Add(Rect(mx, btnY - 4, 8, 8, "none", _colors.TitleBarText));

        // Minimize (─)
        var minX = mx - 25;
        svg.Add(Line(minX, btnY, minX + 8, btnY, _colors.TitleBarText));
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
            case "button": RenderButton(svg, c, ax, ay); break;
            case "textbox": RenderTextBox(svg, c, ax, ay); break;
            case "label": RenderStaticLabel(svg, c, ax, ay); break;
            case "combobox": RenderComboBox(svg, c, ax, ay); break;
            case "checkbox": RenderCheckBox(svg, c, ax, ay); break;
            case "radiobutton": RenderRadioButton(svg, c, ax, ay); break;
            case "group": RenderGroupBox(svg, c, ax, ay); break;
            case "datagrid": RenderDataGrid(svg, c, ax, ay); break;
            case "menubar": RenderMenuBar(svg, c, ax, ay); break;
            case "statusbar": RenderStatusBar(svg, c, ax, ay); break;
            case "tabcontrol": RenderTabControl(svg, c, ax, ay); break;
            case "listbox": RenderListBox(svg, c, ax, ay); break;
            case "panel": RenderPanel(svg, c, ax, ay); break;
            case "image": RenderImagePlaceholder(svg, c, ax, ay); break;
            case "progressbar": RenderProgressBar(svg, c, ax, ay); break;
            case "numericupdown": RenderNumericUpDown(svg, c, ax, ay); break;
            case "datetimepicker": RenderDateTimePicker(svg, c, ax, ay); break;
            case "treeview": RenderTreeView(svg, c, ax, ay); break;
            case "toolbar": RenderToolbar(svg, c, ax, ay); break;
            case "linklabel": RenderLinkLabel(svg, c, ax, ay); break;
            case "textarea": RenderTextArea(svg, c, ax, ay); break;
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

    private void RenderButton(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, _colors.ButtonBackground);
        var fg = ResolveColor(c.Foreground, _colors.ButtonText);
        var border = ResolveColor(c.BorderColor, _colors.ButtonBorder);

        svg.Add(Rect(x, y, c.Width, c.Height, bg, border, 1, 3));
        if (!string.IsNullOrEmpty(c.Text))
            svg.Add(Txt(c.Text, x + c.Width / 2, y + c.Height / 2, 12, fg, "middle"));
    }

    private void RenderTextBox(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, _colors.TextBoxBackground);
        var border = ResolveColor(c.BorderColor, _colors.TextBoxBorder);

        svg.Add(Rect(x, y, c.Width, c.Height, bg, border, 1, 2));

        var display = !string.IsNullOrEmpty(c.Text) ? c.Text : c.Placeholder;
        var fill = !string.IsNullOrEmpty(c.Text) ? (ResolveColor(c.Foreground, _colors.TextBoxText)) : _colors.TextBoxPlaceholder;
        var style = string.IsNullOrEmpty(c.Text) && !string.IsNullOrEmpty(c.Placeholder) ? "italic" : null;

        if (!string.IsNullOrEmpty(display))
            svg.Add(Txt(display, x + 4, y + c.Height / 2, 12, fill, fontStyle: style));
    }

    private void RenderStaticLabel(XElement svg, ControlDefinition c, int x, int y)
    {
        if (!string.IsNullOrEmpty(c.Text))
        {
            var fg = ResolveColor(c.Foreground, _colors.LabelText);
            svg.Add(Txt(c.Text, x, y + c.Height / 2, 12, fg));
        }
    }

    private void RenderComboBox(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, _colors.TextBoxBackground);
        var fg = ResolveColor(c.Foreground, _colors.TextBoxText);
        var border = ResolveColor(c.BorderColor, _colors.TextBoxBorder);

        svg.Add(Rect(x, y, c.Width, c.Height, bg, border, 1, 2));

        var display = c.Text ?? c.Items?.FirstOrDefault() ?? "";
        if (!string.IsNullOrEmpty(display))
            svg.Add(Txt(display, x + 4, y + c.Height / 2, 12, fg));

        // ドロップダウンボタン
        svg.Add(Rect(x + c.Width - 20, y, 20, c.Height, _colors.ButtonBackground, border, 1, 2));
        var am = x + c.Width - 10;
        var arrowY = y + c.Height / 2 - 3;
        svg.Add(El("polygon",
            At("points", $"{x + c.Width - 15},{arrowY} {x + c.Width - 5},{arrowY} {am},{arrowY + 6}"),
            At("fill", "#666666")));
    }

    private void RenderCheckBox(XElement svg, ControlDefinition c, int x, int y)
    {
        var fg = ResolveColor(c.Foreground, _colors.LabelText);

        var boxSize = 14;
        var boxY = y + 3;
        svg.Add(Rect(x, boxY, boxSize, boxSize, _colors.TextBoxBackground, "#666666", 1, 2));

        if (c.Checked)
        {
            svg.Add(El("polyline",
                At("points", $"{x + 3},{boxY + 7} {x + 6},{boxY + 10} {x + 11},{boxY + 3}"),
                At("fill", "none"),
                At("stroke", _colors.TitleBarBackground),
                At("stroke-width", 2)));
        }

        if (!string.IsNullOrEmpty(c.Text))
            svg.Add(Txt(c.Text, x + boxSize + 6, y + 10, 12, fg));
    }

    private void RenderRadioButton(XElement svg, ControlDefinition c, int x, int y)
    {
        var fg = ResolveColor(c.Foreground, _colors.LabelText);

        var r = 7;
        var cx = x + r;
        var cy = y + 10;
        svg.Add(El("circle", At("cx", cx), At("cy", cy), At("r", r),
            At("fill", _colors.TextBoxBackground), At("stroke", "#666666"), At("stroke-width", 1)));

        if (c.Selected)
            svg.Add(El("circle", At("cx", cx), At("cy", cy), At("r", 4),
                At("fill", _colors.TitleBarBackground)));

        if (!string.IsNullOrEmpty(c.Text))
            svg.Add(Txt(c.Text, x + r * 2 + 6, y + 10, 12, fg));
    }

    private void RenderGroupBox(XElement svg, ControlDefinition c, int ax, int ay)
    {
        var border = ResolveColor(c.BorderColor, _colors.GroupBorder);
        var fg = ResolveColor(c.Foreground, _colors.GroupText);
        var w = c.Width;
        var h = c.Height;

        // 枠線（ラベル分下げる）
        svg.Add(Rect(ax, ay + 8, w, h - 8, "none", border, 1, 3));

        // ラベル背景 + テキスト
        var labelW = EstimateTextWidth(c.Text, 11) + 12;
        svg.Add(Rect(ax + 8, ay, labelW, 16, _colors.WindowBackground, "none"));
        if (!string.IsNullOrEmpty(c.Text))
            svg.Add(Txt(c.Text, ax + 14, ay + 8, 11, fg));

        // 子コントロール
        if (c.Controls != null)
        {
            foreach (var child in c.Controls)
                RenderControl(svg, child, ax, ay + 16, w, h - 16);
        }
    }

    private void RenderDataGrid(XElement svg, ControlDefinition c, int ax, int ay)
    {
        var bg = ResolveColor(c.Background, _colors.GridBackground);
        var border = ResolveColor(c.BorderColor, _colors.GridBorder);
        var w = c.Width;
        var h = c.Height;
        var headerH = 26;
        var rowH = 24;

        svg.Add(Rect(ax, ay, w, h, bg, border));
        svg.Add(Rect(ax, ay, w, headerH, _colors.GridHeaderBackground, border));

        if (c.Columns != null)
        {
            var colX = ax;
            for (var i = 0; i < c.Columns.Count; i++)
            {
                var col = c.Columns[i];
                var colW = col.Width > 0 ? col.Width : w / c.Columns.Count;

                svg.Add(Txt(col.Header, colX + 8, ay + headerH / 2, 11,
                    _colors.GridHeaderText, weight: "bold"));

                if (i > 0)
                    svg.Add(Line(colX, ay, colX, ay + h, border));

                colX += colW;
            }
        }

        svg.Add(Line(ax, ay + headerH, ax + w, ay + headerH, border));

        if (c.Rows != null && c.Columns != null)
        {
            for (var ri = 0; ri < c.Rows.Count; ri++)
            {
                var row = c.Rows[ri];
                var rowY = ay + headerH + ri * rowH;
                if (rowY + rowH > ay + h) break;

                if (ri % 2 == 1)
                    svg.Add(Rect(ax + 1, rowY, w - 2, rowH, _colors.GridRowAlternate, "none"));

                svg.Add(Line(ax, rowY, ax + w, rowY, border));

                var colX = ax;
                for (var ci = 0; ci < Math.Min(row.Count, c.Columns.Count); ci++)
                {
                    var colW = c.Columns[ci].Width > 0 ? c.Columns[ci].Width : w / c.Columns.Count;
                    svg.Add(Txt(row[ci], colX + 8, rowY + rowH / 2, 11, ResolveColor(c.Foreground, _colors.GridCellText)));
                    colX += colW;
                }
            }
        }
    }

    private void RenderMenuBar(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, _colors.MenuBackground);
        var fg = ResolveColor(c.Foreground, _colors.MenuText);

        svg.Add(Rect(x, y, c.Width, c.Height, bg, "none"));
        svg.Add(Line(x, y + c.Height, x + c.Width, y + c.Height, _colors.MenuBorder));

        if (c.Items != null)
        {
            var itemX = x + 8;
            foreach (var item in c.Items)
            {
                svg.Add(Txt(item, itemX, y + c.Height / 2, 12, fg));
                itemX += EstimateTextWidth(item, 12) + 16;
            }
        }
    }

    private void RenderStatusBar(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, _colors.StatusBarBackground);
        var fg = ResolveColor(c.Foreground, _colors.StatusBarText);

        svg.Add(Rect(x, y, c.Width, c.Height, bg, "none"));

        if (c.Items != null)
        {
            var itemX = x + 10;
            for (var i = 0; i < c.Items.Count; i++)
            {
                svg.Add(Txt(c.Items[i], itemX, y + c.Height / 2, 11, fg));
                itemX += EstimateTextWidth(c.Items[i], 11) + 20;

                if (i < c.Items.Count - 1)
                    svg.Add(Line(itemX - 10, y + 3, itemX - 10, y + c.Height - 3, "#FFFFFF"));
            }
        }
    }

    private void RenderTabControl(XElement svg, ControlDefinition c, int ax, int ay)
    {
        var border = ResolveColor(c.BorderColor, _colors.GridBorder);
        var tabH = 26;
        var w = c.Width;
        var h = c.Height;

        svg.Add(Rect(ax, ay + tabH, w, h - tabH, _colors.TextBoxBackground, border));

        if (c.Tabs != null)
        {
            var tabX = ax;
            for (var i = 0; i < c.Tabs.Count; i++)
            {
                var tab = c.Tabs[i];
                var tabW = EstimateTextWidth(tab.Text, 12) + 24;
                var isActive = i == c.ActiveTab;
                var tabFill = isActive ? _colors.TextBoxBackground : _colors.GridHeaderBackground;

                svg.Add(Rect(tabX, ay, tabW, tabH + 1, tabFill, border, 1, 3));
                if (isActive)
                    svg.Add(Rect(tabX + 1, ay + tabH - 1, tabW - 2, 3, _colors.TextBoxBackground, "none"));

                svg.Add(Txt(tab.Text, tabX + 12, ay + tabH / 2, 12,
                    isActive ? _colors.LabelText : _colors.GroupText));

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

    private void RenderListBox(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, _colors.TextBoxBackground);
        var border = ResolveColor(c.BorderColor, _colors.TextBoxBorder);

        svg.Add(Rect(x, y, c.Width, c.Height, bg, border));

        if (c.Items != null)
        {
            var itemH = 22;
            for (var i = 0; i < c.Items.Count; i++)
            {
                var itemY = y + i * itemH;
                if (itemY + itemH > y + c.Height) break;

                if (i == c.SelectedIndex)
                    svg.Add(Rect(x + 1, itemY, c.Width - 2, itemH, "#0078D4", "none"));

                svg.Add(Txt(c.Items[i], x + 6, itemY + itemH / 2, 12,
                    i == c.SelectedIndex ? "#FFFFFF" : (ResolveColor(c.Foreground, _colors.LabelText))));
            }
        }
    }

    private void RenderPanel(XElement svg, ControlDefinition c, int ax, int ay)
    {
        var border = ResolveColor(c.BorderColor, _colors.GridBorder);

        svg.Add(Rect(ax, ay, c.Width, c.Height, "none", border, 1, 2));
        if (c.Controls != null)
        {
            foreach (var child in c.Controls)
                RenderControl(svg, child, ax, ay, c.Width, c.Height);
        }
    }

    private void RenderImagePlaceholder(XElement svg, ControlDefinition c, int x, int y)
    {
        svg.Add(Rect(x, y, c.Width, c.Height, "#F5F5F5", _colors.GridBorder, 1, 2));

        var cx = x + c.Width / 2;
        var cy = y + c.Height / 2 - 10;
        svg.Add(El("polygon",
            At("points", $"{cx - 20},{cy + 15} {cx - 5},{cy - 5} {cx + 5},{cy + 5} {cx + 20},{cy + 15}"),
            At("fill", "#C0C0C0")));
        svg.Add(El("circle", At("cx", cx + 10), At("cy", cy - 8), At("r", 5), At("fill", "#C0C0C0")));

        var label = !string.IsNullOrEmpty(c.Text) ? c.Text : "画像";
        svg.Add(Txt(label, cx, cy + 30, 11, "#999999", "middle"));
    }

    private void RenderProgressBar(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, "#E0E0E0");
        var fg = ResolveColor(c.Foreground, _colors.TitleBarBackground);

        var clamped = Math.Clamp(c.Value, 0, 100);
        svg.Add(Rect(x, y, c.Width, c.Height, bg, _colors.GridBorder, 1, 3));

        var fillW = (int)((c.Width - 2) * clamped / 100.0);
        if (fillW > 0)
            svg.Add(Rect(x + 1, y + 1, fillW, c.Height - 2, fg, "none", 0, 2));

        svg.Add(Txt($"{clamped}%", x + c.Width / 2, y + c.Height / 2, 10, _colors.LabelText, "middle", "bold"));
    }

    private void RenderNumericUpDown(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, _colors.TextBoxBackground);
        var fg = ResolveColor(c.Foreground, _colors.TextBoxText);
        var border = ResolveColor(c.BorderColor, _colors.TextBoxBorder);

        var clamped = Math.Clamp(c.Value, c.Minimum, c.Maximum);
        var btnW = 18;

        // テキストエリア
        svg.Add(Rect(x, y, c.Width, c.Height, bg, border, 1, 2));
        svg.Add(Txt(clamped.ToString(CultureInfo.InvariantCulture), x + 4, y + c.Height / 2, 12, fg));

        // 上ボタン
        svg.Add(Rect(x + c.Width - btnW, y, btnW, c.Height / 2, _colors.ButtonBackground, border, 1, 0));
        var upX = x + c.Width - btnW / 2;
        var upY = y + c.Height / 4;
        svg.Add(El("polygon",
            At("points", $"{upX - 4},{upY + 2} {upX + 4},{upY + 2} {upX},{upY - 3}"),
            At("fill", "#666666")));

        // 下ボタン
        svg.Add(Rect(x + c.Width - btnW, y + c.Height / 2, btnW, c.Height / 2, _colors.ButtonBackground, border, 1, 0));
        var dnY = y + c.Height * 3 / 4;
        svg.Add(El("polygon",
            At("points", $"{upX - 4},{dnY - 2} {upX + 4},{dnY - 2} {upX},{dnY + 3}"),
            At("fill", "#666666")));
    }

    private void RenderDateTimePicker(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, _colors.TextBoxBackground);
        var fg = ResolveColor(c.Foreground, _colors.TextBoxText);
        var border = ResolveColor(c.BorderColor, _colors.TextBoxBorder);

        svg.Add(Rect(x, y, c.Width, c.Height, bg, border, 1, 2));

        var display = c.DateText;
        if (string.IsNullOrEmpty(display))
        {
            display = (c.Format?.ToLowerInvariant()) switch
            {
                "time" => "00:00:00",
                "long" => "2026年1月1日",
                _ => "2026/01/01"
            };
        }

        svg.Add(Txt(display, x + 4, y + c.Height / 2, 12, fg));

        // カレンダーボタン
        var btnW = 20;
        svg.Add(Rect(x + c.Width - btnW, y, btnW, c.Height, _colors.ButtonBackground, border, 1, 2));

        // カレンダーアイコン（簡略化）
        var ix = x + c.Width - btnW / 2;
        var iy = y + c.Height / 2;
        svg.Add(Rect(ix - 5, iy - 5, 10, 10, "none", "#666666", 1, 1));
        svg.Add(Line(ix - 5, iy - 2, ix + 5, iy - 2, "#666666"));
        svg.Add(El("circle", At("cx", ix), At("cy", iy + 2), At("r", 1), At("fill", "#666666")));
    }

    private void RenderTreeView(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, _colors.TreeViewBackground);
        var border = ResolveColor(c.BorderColor, _colors.TextBoxBorder);

        svg.Add(Rect(x, y, c.Width, c.Height, bg, border));

        if (c.Nodes != null)
        {
            var currentY = y + 4;
            foreach (var node in c.Nodes)
                currentY = RenderTreeNode(svg, node, x + 6, currentY, 0, y + c.Height);
        }
    }

    private int RenderTreeNode(XElement svg, TreeNodeDefinition node, int x, int y,
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
                    At("fill", _colors.TreeViewExpander)));
            }
            else
            {
                // ▶
                svg.Add(El("polygon",
                    At("points", $"{nx},{y + 4} {nx},{y + 14} {nx + 7},{y + 9}"),
                    At("fill", _colors.TreeViewExpander)));
            }
        }

        // ノードテキスト
        svg.Add(Txt(node.Text, nx + 14, y + nodeH / 2, 12, _colors.TreeViewText));

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

    private void RenderToolbar(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, _colors.ToolbarBackground);
        var fg = ResolveColor(c.Foreground, _colors.ToolbarText);

        svg.Add(Rect(x, y, c.Width, c.Height, bg, "none"));
        svg.Add(Line(x, y + c.Height, x + c.Width, y + c.Height, _colors.ToolbarBorder));

        if (c.Items != null)
        {
            var itemX = x + 4;
            foreach (var item in c.Items)
            {
                if (item == "|" || item == "-")
                {
                    // セパレータ
                    svg.Add(Line(itemX + 2, y + 4, itemX + 2, y + c.Height - 4, _colors.ToolbarSeparator));
                    itemX += 8;
                }
                else
                {
                    var btnW = EstimateTextWidth(item, 11) + 12;
                    svg.Add(Rect(itemX, y + 2, btnW, c.Height - 4, "none", "none", 0, 3));
                    svg.Add(Txt(item, itemX + 6, y + c.Height / 2, 11, fg));
                    itemX += btnW + 2;
                }
            }
        }
    }

    private void RenderLinkLabel(XElement svg, ControlDefinition c, int x, int y)
    {
        if (!string.IsNullOrEmpty(c.Text))
        {
            var fg = ResolveColor(c.Foreground, _colors.LinkLabelText);
            var textW = EstimateTextWidth(c.Text, 12);
            svg.Add(Txt(c.Text, x, y + c.Height / 2, 12, fg));
            svg.Add(Line(x, y + c.Height / 2 + 8, x + textW, y + c.Height / 2 + 8, _colors.LinkLabelUnderline));
        }
    }

    private void RenderTextArea(XElement svg, ControlDefinition c, int x, int y)
    {
        var bg = ResolveColor(c.Background, c.ReadOnly ? "#F5F5F5" : _colors.TextBoxBackground);
        var border = ResolveColor(c.BorderColor, _colors.TextBoxBorder);

        svg.Add(Rect(x, y, c.Width, c.Height, bg, border, 1, 2));

        // スクロールバー
        var scrollW = 14;
        svg.Add(Rect(x + c.Width - scrollW, y + 1, scrollW - 1, c.Height - 2, "#F0F0F0", _colors.GridBorder, 1, 0));
        // スクロールサム
        svg.Add(Rect(x + c.Width - scrollW + 2, y + 4, scrollW - 5, 20, "#C0C0C0", "none", 0, 3));

        var display = !string.IsNullOrEmpty(c.Text) ? c.Text : c.Placeholder;
        var fill = !string.IsNullOrEmpty(c.Text) ? (ResolveColor(c.Foreground, _colors.TextBoxText)) : _colors.TextBoxPlaceholder;
        var style = string.IsNullOrEmpty(c.Text) && !string.IsNullOrEmpty(c.Placeholder) ? "italic" : null;

        if (!string.IsNullOrEmpty(display))
        {
            // 複数行表示（改行で分割）
            var lines = display.Split('\n');
            var lineH = 18;
            for (var i = 0; i < lines.Length; i++)
            {
                var ly = y + 12 + i * lineH;
                if (ly > y + c.Height - 6) break;
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

            // 個別色オーバーライド
            var lineColor = ResolveColor(ann.LineColor, _colors.AnnotationLine);
            var circleColor = ResolveColor(ann.LabelBackground, _colors.AnnotationCircle);
            var textColor = ResolveColor(ann.LabelColor, _colors.AnnotationText);
            var dashArray = ResolveDashArray(ann.LineStyle, "dashed");

            // リーダー線
            var lineStartX = target.Right + 2;
            var lineStartY = target.CenterY;
            var lineEndX = circleX - Theme.AnnotationRadius - 2;

            svg.Add(Line(lineStartX, lineStartY, lineEndX, circleY,
                lineColor, 1, dashArray));

            // コントロール接続点
            svg.Add(El("circle",
                At("cx", lineStartX), At("cy", lineStartY), At("r", 3),
                At("fill", lineColor)));

            // ラベル円
            var radius = Math.Max(Theme.AnnotationRadius, EstimateTextWidth(ann.Label, 13) / 2 + 4);
            svg.Add(El("circle",
                At("cx", circleX), At("cy", circleY), At("r", radius),
                At("fill", circleColor)));

            svg.Add(Txt(ann.Label, circleX, circleY, 13,
                textColor, "middle", "bold"));
        }
    }

    /// <summary>線スタイル文字列からSVGのstroke-dasharrayを解決する</summary>
    private static string? ResolveDashArray(string? lineStyle, string defaultStyle)
    {
        var style = (lineStyle ?? defaultStyle).ToLowerInvariant();
        return style switch
        {
            "solid" => null,
            "dotted" => "2,3",
            _ => "4,3" // dashed
        };
    }

    // ────────────────────────────────────────────
    //  コネクタ描画
    // ────────────────────────────────────────────

    private void RenderConnectors(XElement svg, List<ConnectorDefinition> connectors)
    {
        foreach (var conn in connectors)
        {
            if (!_bounds.TryGetValue(conn.From, out var fromBounds) ||
                !_bounds.TryGetValue(conn.To, out var toBounds))
                continue;

            var lineColor = ResolveColor(conn.LineColor, _colors.ConnectorLine);
            var circleColor = ResolveColor(conn.LabelBackground, _colors.ConnectorCircle);
            var textColor = ResolveColor(conn.LabelColor, _colors.ConnectorText);
            var dashArray = ResolveDashArray(conn.LineStyle, "solid");

            // 最近接エッジ間を結ぶ座標を計算
            var (startX, startY, endX, endY) = ComputeEdgePoints(fromBounds, toBounds);

            // コネクタ線
            svg.Add(Line(startX, startY, endX, endY, lineColor, 1, dashArray));

            // 接続元の端点形状
            var fromShape = (conn.FromShape ?? "none").ToLowerInvariant();
            RenderEndpointShape(svg, fromShape, endX, endY, startX, startY, lineColor);

            // 接続先の端点形状
            var toShape = (conn.ToShape ?? "arrow").ToLowerInvariant();
            RenderEndpointShape(svg, toShape, startX, startY, endX, endY, lineColor);

            // ラベル（中間点に表示）
            if (!string.IsNullOrEmpty(conn.Label))
            {
                var midX = (startX + endX) / 2;
                var midY = (startY + endY) / 2;
                var radius = Math.Max(Theme.AnnotationRadius, EstimateTextWidth(conn.Label, 13) / 2 + 4);

                // ラベル背景円
                svg.Add(El("circle",
                    At("cx", midX), At("cy", midY), At("r", radius),
                    At("fill", circleColor)));

                svg.Add(Txt(conn.Label, midX, midY, 13,
                    textColor, "middle", "bold"));
            }
        }
    }

    /// <summary>端点形状を描画する。形状は始点→終点方向の端点に描画される</summary>
    private static void RenderEndpointShape(XElement svg, string shape,
        int fromX, int fromY, int toX, int toY, string color)
    {
        switch (shape)
        {
            case "arrow":
                RenderArrowHead(svg, fromX, fromY, toX, toY, color);
                break;
            case "circle":
                svg.Add(El("circle",
                    At("cx", toX), At("cy", toY), At("r", 4),
                    At("fill", color)));
                break;
            case "diamond":
                RenderDiamond(svg, fromX, fromY, toX, toY, color);
                break;
            case "square":
                RenderSquare(svg, toX, toY, color);
                break;
            // "none" の場合は何も描画しない
        }
    }

    /// <summary>ひし形の端点を描画する</summary>
    private static void RenderDiamond(XElement svg, int x1, int y1, int x2, int y2, string color)
    {
        var dx = x2 - x1;
        var dy = y2 - y1;
        var len = Math.Sqrt(dx * dx + dy * dy);
        if (len < 1) return;

        var ux = dx / len;
        var uy = dy / len;
        const int size = 6;

        // ひし形の4頂点: 先端, 左, 後端, 右
        var tipX = x2;
        var tipY = y2;
        var backX = (int)(x2 - ux * size * 2);
        var backY = (int)(y2 - uy * size * 2);
        var midX = (int)(x2 - ux * size);
        var midY = (int)(y2 - uy * size);
        var px = (int)(-uy * size);
        var py = (int)(ux * size);

        svg.Add(El("polygon",
            At("points", $"{tipX},{tipY} {midX + px},{midY + py} {backX},{backY} {midX - px},{midY - py}"),
            At("fill", color)));
    }

    /// <summary>四角形の端点を描画する</summary>
    private static void RenderSquare(XElement svg, int x, int y, string color)
    {
        const int half = 4;
        svg.Add(El("rect",
            At("x", x - half), At("y", y - half),
            At("width", half * 2), At("height", half * 2),
            At("fill", color)));
    }

    /// <summary>2つのコントロールの最近接エッジ間の座標を計算する</summary>
    private static (int StartX, int StartY, int EndX, int EndY) ComputeEdgePoints(
        ControlBounds from, ControlBounds to)
    {
        var fromCx = from.X + from.Width / 2;
        var fromCy = from.Y + from.Height / 2;
        var toCx = to.X + to.Width / 2;
        var toCy = to.Y + to.Height / 2;

        var startX = ClampToEdgeX(from, toCx, toCy);
        var startY = ClampToEdgeY(from, toCx, toCy);
        var endX = ClampToEdgeX(to, fromCx, fromCy);
        var endY = ClampToEdgeY(to, fromCx, fromCy);

        return (startX, startY, endX, endY);
    }

    private static int ClampToEdgeX(ControlBounds b, int targetX, int targetY)
    {
        var cx = b.X + b.Width / 2;
        var cy = b.Y + b.Height / 2;
        var dx = targetX - cx;
        var dy = targetY - cy;
        if (dx == 0 && dy == 0) return cx;

        var scaleX = b.Width / 2.0 / Math.Max(1, Math.Abs(dx));
        var scaleY = b.Height / 2.0 / Math.Max(1, Math.Abs(dy));
        var scale = Math.Min(scaleX, scaleY);
        return cx + (int)(dx * scale);
    }

    private static int ClampToEdgeY(ControlBounds b, int targetX, int targetY)
    {
        var cx = b.X + b.Width / 2;
        var cy = b.Y + b.Height / 2;
        var dx = targetX - cx;
        var dy = targetY - cy;
        if (dx == 0 && dy == 0) return cy;

        var scaleX = b.Width / 2.0 / Math.Max(1, Math.Abs(dx));
        var scaleY = b.Height / 2.0 / Math.Max(1, Math.Abs(dy));
        var scale = Math.Min(scaleX, scaleY);
        return cy + (int)(dy * scale);
    }

    /// <summary>接続先に矢印を描画する</summary>
    private static void RenderArrowHead(XElement svg, int x1, int y1, int x2, int y2, string color)
    {
        var dx = x2 - x1;
        var dy = y2 - y1;
        var len = Math.Sqrt(dx * dx + dy * dy);
        if (len < 1) return;

        var ux = dx / len;
        var uy = dy / len;
        const int arrowSize = 8;

        var ax = x2 - (int)(ux * arrowSize);
        var ay = y2 - (int)(uy * arrowSize);
        var px = (int)(-uy * arrowSize * 0.4);
        var py = (int)(ux * arrowSize * 0.4);

        svg.Add(El("polygon",
            At("points", $"{x2},{y2} {ax + px},{ay + py} {ax - px},{ay - py}"),
            At("fill", color)));
    }
}
