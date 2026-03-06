import { Theme } from "./themeColors";
import type { ThemeColors } from "./themeColors";
import type {
    ScreenDefinition,
    WindowDefinition,
    ControlDefinition,
    AnnotationDefinition,
    TreeNodeDefinition,
} from "./models";
import {
    applyWindowDefaults,
    applyControlDefaults,
    applyColumnDefaults,
    applyTreeNodeDefaults,
} from "./models";

// ────────────────────────────────────────────
//  XML escape
// ────────────────────────────────────────────

function esc(s: string): string {
    return s
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;");
}

// ────────────────────────────────────────────
//  SVG string helpers
// ────────────────────────────────────────────

function attr(name: string, val: string | number): string {
    return ` ${name}="${esc(String(val))}"`;
}

function openTag(name: string, attrs: string, selfClose = false): string {
    return `<${name}${attrs}${selfClose ? "/>" : ">"}`;
}

function el(name: string, attrs: string, ...children: string[]): string {
    if (children.length === 0) {
        return openTag(name, attrs, true);
    }
    return `<${name}${attrs}>${children.join("")}</${name}>`;
}

function txt(
    text: string,
    x: number,
    y: number,
    fontSize: number,
    fill: string,
    anchor = "start",
    weight = "normal",
    fontStyle?: string,
): string {
    let a =
        attr("x", x) +
        attr("y", y) +
        attr("font-size", fontSize) +
        attr("fill", fill) +
        attr("text-anchor", anchor) +
        attr("font-weight", weight) +
        attr("dominant-baseline", "central");
    if (fontStyle) a += attr("font-style", fontStyle);
    return el("text", a, esc(text));
}

function rect(
    x: number,
    y: number,
    w: number,
    h: number,
    fill: string,
    stroke: string,
    strokeWidth = 1,
    rx = 0,
    filter?: string,
): string {
    let a =
        attr("x", x) +
        attr("y", y) +
        attr("width", w) +
        attr("height", h) +
        attr("fill", fill) +
        attr("stroke", stroke) +
        attr("stroke-width", strokeWidth);
    if (rx > 0) a += attr("rx", rx);
    if (filter) a += attr("filter", filter);
    return openTag("rect", a, true);
}

function line(
    x1: number,
    y1: number,
    x2: number,
    y2: number,
    stroke: string,
    strokeWidth = 1,
    dashArray?: string,
): string {
    let a =
        attr("x1", x1) +
        attr("y1", y1) +
        attr("x2", x2) +
        attr("y2", y2) +
        attr("stroke", stroke) +
        attr("stroke-width", strokeWidth);
    if (dashArray) a += attr("stroke-dasharray", dashArray);
    return openTag("line", a, true);
}

function estimateTextWidth(text: string | undefined, fontSize: number): number {
    if (!text) return 0;
    let width = 0;
    for (let i = 0; i < text.length; i++) {
        const code = text.codePointAt(i)!;
        // 0x2E80 marks the start of CJK character ranges (full-width).
        // 0.55 approximates the average width of ASCII characters relative to font size.
        width += code > 0x2e80 ? fontSize : Math.round(fontSize * 0.55);
        // Skip low surrogate of surrogate pair
        if (code > 0xffff) i++;
    }
    return width;
}

function clamp(val: number, min: number, max: number): number {
    return Math.max(min, Math.min(val, max));
}

// ────────────────────────────────────────────
//  Control bounds tracking
// ────────────────────────────────────────────

interface ControlBounds {
    x: number;
    y: number;
    width: number;
    height: number;
    centerY: number;
    right: number;
}

function makeBounds(x: number, y: number, w: number, h: number): ControlBounds {
    return { x, y, width: w, height: h, centerY: y + Math.floor(h / 2), right: x + w };
}

// ────────────────────────────────────────────
//  SvgRenderer
// ────────────────────────────────────────────

export class SvgRenderer {
    private readonly _bounds = new Map<string, ControlBounds>();
    private readonly _colors: ThemeColors;

    constructor(colors: ThemeColors) {
        this._colors = colors;
    }

    // ── Public API ──

    render(def: ScreenDefinition): string {
        this._bounds.clear();

        const window = applyWindowDefaults(def.window);
        const hasAnnotations = !!def.annotations && def.annotations.length > 0;
        const annotationMargin = hasAnnotations ? Theme.AnnotationMargin : 0;
        const pad = window.chrome ? Theme.WindowPadding : Theme.ChromelessPadding;

        const svgWidth = window.width + pad * 2 + annotationMargin;
        const svgHeight = window.height + pad * 2;

        const parts: string[] = [];
        parts.push(this.createDefs());
        parts.push(rect(0, 0, svgWidth, svgHeight, this._colors.CanvasBackground, "none", 0));

        if (window.chrome) {
            this.renderWindow(parts, window, pad, pad);
        } else {
            this.renderChromeless(parts, window, pad, pad);
        }

        if (def.annotations && def.annotations.length > 0) {
            this.renderAnnotations(parts, def.annotations, pad, pad, window.width);
        }

        const svgAttrs =
            attr("xmlns", "http://www.w3.org/2000/svg") +
            attr("width", svgWidth) +
            attr("height", svgHeight) +
            attr("viewBox", `0 0 ${svgWidth} ${svgHeight}`);

        return `<?xml version="1.0" encoding="UTF-8"?>\n${el("svg", svgAttrs, ...parts)}`;
    }

    // ── Defs ──

    private createDefs(): string {
        const filter = el(
            "filter",
            attr("id", "windowShadow"),
            el(
                "feDropShadow",
                attr("dx", 2) +
                    attr("dy", 2) +
                    attr("stdDeviation", 4) +
                    attr("flood-opacity", 0.15),
            ),
        );
        const style = el("style", "", `text { font-family: ${Theme.FontFamily}; }`);
        return el("defs", "", filter, style);
    }

    // ── Window ──

    private renderChromeless(
        parts: string[],
        window: WindowDefinition,
        wx: number,
        wy: number,
    ): void {
        parts.push(
            rect(wx, wy, window.width, window.height, this._colors.WindowBackground, this._colors.ChromelessBorder, 1, 2),
        );
        if (window.controls) {
            for (const c of window.controls) {
                this.renderControl(parts, c, wx, wy, window.width, window.height);
            }
        }
    }

    private renderWindow(
        parts: string[],
        window: WindowDefinition,
        wx: number,
        wy: number,
    ): void {
        // Window background + shadow
        parts.push(
            rect(
                wx, wy, window.width, window.height,
                this._colors.WindowBackground, this._colors.WindowBorder,
                1, 4, "url(#windowShadow)",
            ),
        );

        // Title bar (rounded top)
        parts.push(
            rect(wx, wy, window.width, Theme.TitleBarHeight, this._colors.TitleBarBackground, "none", 0, 4),
        );
        parts.push(
            rect(wx, wy + Theme.TitleBarHeight - 6, window.width, 6, this._colors.TitleBarBackground, "none"),
        );

        // Title text
        parts.push(
            txt(window.title ?? "", wx + 10, wy + Math.floor(Theme.TitleBarHeight / 2), 12, this._colors.TitleBarText),
        );

        // Window buttons
        this.renderWindowButtons(parts, wx, wy, window.width);

        // Content area
        const contentX = wx;
        const contentY = wy + Theme.TitleBarHeight;
        const contentW = window.width;
        const contentH = window.height - Theme.TitleBarHeight;

        if (window.controls) {
            for (const c of window.controls) {
                this.renderControl(parts, c, contentX, contentY, contentW, contentH);
            }
        }
    }

    private renderWindowButtons(parts: string[], wx: number, wy: number, windowWidth: number): void {
        const btnY = wy + Math.floor(Theme.TitleBarHeight / 2);

        // Close (×)
        const cx = wx + windowWidth - 22;
        parts.push(line(cx, btnY - 4, cx + 8, btnY + 4, this._colors.TitleBarText));
        parts.push(line(cx, btnY + 4, cx + 8, btnY - 4, this._colors.TitleBarText));

        // Maximize (□)
        const mx = cx - 25;
        parts.push(rect(mx, btnY - 4, 8, 8, "none", this._colors.TitleBarText));

        // Minimize (─)
        const minX = mx - 25;
        parts.push(line(minX, btnY, minX + 8, btnY, this._colors.TitleBarText));
    }

    // ── Control dispatch ──

    private renderControl(
        parts: string[],
        rawC: ControlDefinition | Record<string, unknown>,
        offsetX: number,
        offsetY: number,
        containerW: number,
        containerH: number,
    ): void {
        const c = applyControlDefaults(rawC as Record<string, unknown>);
        this.applyDefaults(c, containerW, containerH);

        const ax = offsetX + c.x;
        const ay = offsetY + c.y;
        this.register(c.id, ax, ay, c.width, c.height);

        switch (c.type.toLowerCase()) {
            case "button": this.renderButton(parts, c, ax, ay); break;
            case "textbox": this.renderTextBox(parts, c, ax, ay); break;
            case "label": this.renderStaticLabel(parts, c, ax, ay); break;
            case "combobox": this.renderComboBox(parts, c, ax, ay); break;
            case "checkbox": this.renderCheckBox(parts, c, ax, ay); break;
            case "radiobutton": this.renderRadioButton(parts, c, ax, ay); break;
            case "group": this.renderGroupBox(parts, c, ax, ay); break;
            case "datagrid": this.renderDataGrid(parts, c, ax, ay); break;
            case "menubar": this.renderMenuBar(parts, c, ax, ay); break;
            case "statusbar": this.renderStatusBar(parts, c, ax, ay); break;
            case "tabcontrol": this.renderTabControl(parts, c, ax, ay); break;
            case "listbox": this.renderListBox(parts, c, ax, ay); break;
            case "panel": this.renderPanel(parts, c, ax, ay); break;
            case "image": this.renderImagePlaceholder(parts, c, ax, ay); break;
            case "progressbar": this.renderProgressBar(parts, c, ax, ay); break;
            case "numericupdown": this.renderNumericUpDown(parts, c, ax, ay); break;
            case "datetimepicker": this.renderDateTimePicker(parts, c, ax, ay); break;
            case "treeview": this.renderTreeView(parts, c, ax, ay); break;
            case "toolbar": this.renderToolbar(parts, c, ax, ay); break;
            case "linklabel": this.renderLinkLabel(parts, c, ax, ay); break;
            case "textarea": this.renderTextArea(parts, c, ax, ay); break;
        }
    }

    private applyDefaults(c: ControlDefinition, containerW: number, containerH: number): void {
        switch (c.type.toLowerCase()) {
            case "menubar":
                if (c.width === 0) c.width = containerW;
                if (c.height === 0) c.height = Theme.MenuBarHeight;
                break;
            case "statusbar":
                if (c.width === 0) c.width = containerW;
                if (c.height === 0) c.height = Theme.StatusBarHeight;
                if (c.x === 0 && c.y === 0) c.y = containerH - c.height;
                break;
            case "button":
                if (c.width === 0) c.width = 80;
                if (c.height === 0) c.height = 26;
                break;
            case "textbox":
            case "combobox":
                if (c.width === 0) c.width = 150;
                if (c.height === 0) c.height = 24;
                break;
            case "checkbox":
            case "radiobutton":
                if (c.width === 0) c.width = estimateTextWidth(c.text, 12) + 22;
                if (c.height === 0) c.height = 20;
                break;
            case "label":
                if (c.height === 0) c.height = 20;
                break;
            case "progressbar":
                if (c.width === 0) c.width = 200;
                if (c.height === 0) c.height = 20;
                break;
            case "numericupdown":
                if (c.width === 0) c.width = 100;
                if (c.height === 0) c.height = 24;
                break;
            case "datetimepicker":
                if (c.width === 0) c.width = 180;
                if (c.height === 0) c.height = 24;
                break;
            case "toolbar":
                if (c.width === 0) c.width = containerW;
                if (c.height === 0) c.height = 28;
                break;
            case "linklabel":
                if (c.height === 0) c.height = 20;
                break;
            case "textarea":
                if (c.width === 0) c.width = 200;
                if (c.height === 0) c.height = 80;
                break;
        }
    }

    private register(id: string | undefined, x: number, y: number, w: number, h: number): void {
        if (id) {
            this._bounds.set(id, makeBounds(x, y, w, h));
        }
    }

    // ── Individual control renderers ──

    private renderButton(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? this._colors.ButtonBackground;
        const fg = c.foreground ?? this._colors.ButtonText;
        const border = c.borderColor ?? this._colors.ButtonBorder;

        parts.push(rect(x, y, c.width, c.height, bg, border, 1, 3));
        if (c.text) {
            parts.push(txt(c.text, x + Math.floor(c.width / 2), y + Math.floor(c.height / 2), 12, fg, "middle"));
        }
    }

    private renderTextBox(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? this._colors.TextBoxBackground;
        const border = c.borderColor ?? this._colors.TextBoxBorder;

        parts.push(rect(x, y, c.width, c.height, bg, border, 1, 2));

        const display = c.text || c.placeholder;
        const fill = c.text ? (c.foreground ?? this._colors.TextBoxText) : this._colors.TextBoxPlaceholder;
        const fontStyle = !c.text && c.placeholder ? "italic" : undefined;

        if (display) {
            parts.push(txt(display, x + 4, y + Math.floor(c.height / 2), 12, fill, "start", "normal", fontStyle));
        }
    }

    private renderStaticLabel(parts: string[], c: ControlDefinition, x: number, y: number): void {
        if (c.text) {
            const fg = c.foreground ?? this._colors.LabelText;
            parts.push(txt(c.text, x, y + Math.floor(c.height / 2), 12, fg));
        }
    }

    private renderComboBox(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? this._colors.TextBoxBackground;
        const fg = c.foreground ?? this._colors.TextBoxText;
        const border = c.borderColor ?? this._colors.TextBoxBorder;

        parts.push(rect(x, y, c.width, c.height, bg, border, 1, 2));

        const display = c.text ?? (c.items && c.items.length > 0 ? c.items[0] : "");
        if (display) {
            parts.push(txt(display, x + 4, y + Math.floor(c.height / 2), 12, fg));
        }

        // Dropdown button
        parts.push(rect(x + c.width - 20, y, 20, c.height, this._colors.ButtonBackground, border, 1, 2));
        const am = x + c.width - 10;
        const arrowY = y + Math.floor(c.height / 2) - 3;
        parts.push(
            el(
                "polygon",
                attr("points", `${x + c.width - 15},${arrowY} ${x + c.width - 5},${arrowY} ${am},${arrowY + 6}`) +
                    attr("fill", "#666666"),
            ),
        );
    }

    private renderCheckBox(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const fg = c.foreground ?? this._colors.LabelText;
        const boxSize = 14;
        const boxY = y + 3;

        parts.push(rect(x, boxY, boxSize, boxSize, this._colors.TextBoxBackground, "#666666", 1, 2));

        if (c.checked) {
            parts.push(
                el(
                    "polyline",
                    attr("points", `${x + 3},${boxY + 7} ${x + 6},${boxY + 10} ${x + 11},${boxY + 3}`) +
                        attr("fill", "none") +
                        attr("stroke", this._colors.TitleBarBackground) +
                        attr("stroke-width", 2),
                ),
            );
        }

        if (c.text) {
            parts.push(txt(c.text, x + boxSize + 6, y + 10, 12, fg));
        }
    }

    private renderRadioButton(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const fg = c.foreground ?? this._colors.LabelText;
        const r = 7;
        const cxVal = x + r;
        const cy = y + 10;

        parts.push(
            el(
                "circle",
                attr("cx", cxVal) + attr("cy", cy) + attr("r", r) +
                    attr("fill", this._colors.TextBoxBackground) +
                    attr("stroke", "#666666") +
                    attr("stroke-width", 1),
            ),
        );

        if (c.selected) {
            parts.push(
                el(
                    "circle",
                    attr("cx", cxVal) + attr("cy", cy) + attr("r", 4) +
                        attr("fill", this._colors.TitleBarBackground),
                ),
            );
        }

        if (c.text) {
            parts.push(txt(c.text, x + r * 2 + 6, y + 10, 12, fg));
        }
    }

    private renderGroupBox(parts: string[], c: ControlDefinition, ax: number, ay: number): void {
        const border = c.borderColor ?? this._colors.GroupBorder;
        const fg = c.foreground ?? this._colors.GroupText;
        const w = c.width;
        const h = c.height;

        parts.push(rect(ax, ay + 8, w, h - 8, "none", border, 1, 3));

        const labelW = estimateTextWidth(c.text, 11) + 12;
        parts.push(rect(ax + 8, ay, labelW, 16, this._colors.WindowBackground, "none"));
        if (c.text) {
            parts.push(txt(c.text, ax + 14, ay + 8, 11, fg));
        }

        if (c.controls) {
            for (const child of c.controls) {
                this.renderControl(parts, child, ax, ay + 16, w, h - 16);
            }
        }
    }

    private renderDataGrid(parts: string[], c: ControlDefinition, ax: number, ay: number): void {
        const bg = c.background ?? this._colors.GridBackground;
        const border = c.borderColor ?? this._colors.GridBorder;
        const w = c.width;
        const h = c.height;
        const headerH = 26;
        const rowH = 24;

        parts.push(rect(ax, ay, w, h, bg, border));
        parts.push(rect(ax, ay, w, headerH, this._colors.GridHeaderBackground, border));

        const columns = c.columns?.map(applyColumnDefaults);

        if (columns) {
            let colX = ax;
            for (let i = 0; i < columns.length; i++) {
                const col = columns[i];
                const colW = col.width > 0 ? col.width : Math.floor(w / columns.length);

                parts.push(
                    txt(col.header, colX + 8, ay + Math.floor(headerH / 2), 11, this._colors.GridHeaderText, "start", "bold"),
                );

                if (i > 0) {
                    parts.push(line(colX, ay, colX, ay + h, border));
                }

                colX += colW;
            }
        }

        parts.push(line(ax, ay + headerH, ax + w, ay + headerH, border));

        if (c.rows && columns) {
            for (let ri = 0; ri < c.rows.length; ri++) {
                const row = c.rows[ri];
                const rowY = ay + headerH + ri * rowH;
                if (rowY + rowH > ay + h) break;

                if (ri % 2 === 1) {
                    parts.push(rect(ax + 1, rowY, w - 2, rowH, this._colors.GridRowAlternate, "none"));
                }

                parts.push(line(ax, rowY, ax + w, rowY, border));

                let colX = ax;
                for (let ci = 0; ci < Math.min(row.length, columns.length); ci++) {
                    const colW = columns[ci].width > 0 ? columns[ci].width : Math.floor(w / columns.length);
                    parts.push(
                        txt(row[ci], colX + 8, rowY + Math.floor(rowH / 2), 11, c.foreground ?? this._colors.GridCellText),
                    );
                    colX += colW;
                }
            }
        }
    }

    private renderMenuBar(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? this._colors.MenuBackground;
        const fg = c.foreground ?? this._colors.MenuText;

        parts.push(rect(x, y, c.width, c.height, bg, "none"));
        parts.push(line(x, y + c.height, x + c.width, y + c.height, this._colors.MenuBorder));

        if (c.items) {
            let itemX = x + 8;
            for (const item of c.items) {
                parts.push(txt(item, itemX, y + Math.floor(c.height / 2), 12, fg));
                itemX += estimateTextWidth(item, 12) + 16;
            }
        }
    }

    private renderStatusBar(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? this._colors.StatusBarBackground;
        const fg = c.foreground ?? this._colors.StatusBarText;

        parts.push(rect(x, y, c.width, c.height, bg, "none"));

        if (c.items) {
            let itemX = x + 10;
            for (let i = 0; i < c.items.length; i++) {
                parts.push(txt(c.items[i], itemX, y + Math.floor(c.height / 2), 11, fg));
                itemX += estimateTextWidth(c.items[i], 11) + 20;

                if (i < c.items.length - 1) {
                    parts.push(line(itemX - 10, y + 3, itemX - 10, y + c.height - 3, "#FFFFFF"));
                }
            }
        }
    }

    private renderTabControl(parts: string[], c: ControlDefinition, ax: number, ay: number): void {
        const border = c.borderColor ?? this._colors.GridBorder;
        const tabH = 26;
        const w = c.width;
        const h = c.height;

        parts.push(rect(ax, ay + tabH, w, h - tabH, this._colors.TextBoxBackground, border));

        if (c.tabs) {
            let tabX = ax;
            for (let i = 0; i < c.tabs.length; i++) {
                const tab = c.tabs[i];
                const tabW = estimateTextWidth(tab.text, 12) + 24;
                const isActive = i === c.activeTab;
                const tabFill = isActive ? this._colors.TextBoxBackground : this._colors.GridHeaderBackground;

                parts.push(rect(tabX, ay, tabW, tabH + 1, tabFill, border, 1, 3));
                if (isActive) {
                    parts.push(rect(tabX + 1, ay + tabH - 1, tabW - 2, 3, this._colors.TextBoxBackground, "none"));
                }

                parts.push(
                    txt(tab.text, tabX + 12, ay + Math.floor(tabH / 2), 12, isActive ? this._colors.LabelText : this._colors.GroupText),
                );

                tabX += tabW + 2;
            }
        }

        if (c.tabs && c.activeTab >= 0 && c.activeTab < c.tabs.length) {
            const activeTab = c.tabs[c.activeTab];
            if (activeTab.controls) {
                for (const child of activeTab.controls) {
                    this.renderControl(parts, child, ax, ay + tabH, w, h - tabH);
                }
            }
        }
    }

    private renderListBox(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? this._colors.TextBoxBackground;
        const border = c.borderColor ?? this._colors.TextBoxBorder;

        parts.push(rect(x, y, c.width, c.height, bg, border));

        if (c.items) {
            const itemH = 22;
            for (let i = 0; i < c.items.length; i++) {
                const itemY = y + i * itemH;
                if (itemY + itemH > y + c.height) break;

                if (i === c.selectedIndex) {
                    parts.push(rect(x + 1, itemY, c.width - 2, itemH, "#0078D4", "none"));
                }

                parts.push(
                    txt(
                        c.items[i],
                        x + 6,
                        itemY + Math.floor(itemH / 2),
                        12,
                        i === c.selectedIndex ? "#FFFFFF" : (c.foreground ?? this._colors.LabelText),
                    ),
                );
            }
        }
    }

    private renderPanel(parts: string[], c: ControlDefinition, ax: number, ay: number): void {
        const border = c.borderColor ?? this._colors.GridBorder;

        parts.push(rect(ax, ay, c.width, c.height, "none", border, 1, 2));
        if (c.controls) {
            for (const child of c.controls) {
                this.renderControl(parts, child, ax, ay, c.width, c.height);
            }
        }
    }

    private renderImagePlaceholder(parts: string[], c: ControlDefinition, x: number, y: number): void {
        parts.push(rect(x, y, c.width, c.height, "#F5F5F5", this._colors.GridBorder, 1, 2));

        const cxVal = x + Math.floor(c.width / 2);
        const cy = y + Math.floor(c.height / 2) - 10;

        parts.push(
            el(
                "polygon",
                attr("points", `${cxVal - 20},${cy + 15} ${cxVal - 5},${cy - 5} ${cxVal + 5},${cy + 5} ${cxVal + 20},${cy + 15}`) +
                    attr("fill", "#C0C0C0"),
            ),
        );
        parts.push(
            el("circle", attr("cx", cxVal + 10) + attr("cy", cy - 8) + attr("r", 5) + attr("fill", "#C0C0C0")),
        );

        const label = c.text || "画像";
        parts.push(txt(label, cxVal, cy + 30, 11, "#999999", "middle"));
    }

    private renderProgressBar(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? "#E0E0E0";
        const fg = c.foreground ?? this._colors.TitleBarBackground;

        const clamped = clamp(c.value, 0, 100);
        parts.push(rect(x, y, c.width, c.height, bg, this._colors.GridBorder, 1, 3));

        const fillW = Math.floor((c.width - 2) * clamped / 100);
        if (fillW > 0) {
            parts.push(rect(x + 1, y + 1, fillW, c.height - 2, fg, "none", 0, 2));
        }

        parts.push(
            txt(`${clamped}%`, x + Math.floor(c.width / 2), y + Math.floor(c.height / 2), 10, this._colors.LabelText, "middle", "bold"),
        );
    }

    private renderNumericUpDown(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? this._colors.TextBoxBackground;
        const fg = c.foreground ?? this._colors.TextBoxText;
        const border = c.borderColor ?? this._colors.TextBoxBorder;

        const clamped = clamp(c.value, c.minimum, c.maximum);
        const btnW = 18;

        // Text area
        parts.push(rect(x, y, c.width, c.height, bg, border, 1, 2));
        parts.push(txt(String(clamped), x + 4, y + Math.floor(c.height / 2), 12, fg));

        // Up button
        parts.push(rect(x + c.width - btnW, y, btnW, Math.floor(c.height / 2), this._colors.ButtonBackground, border, 1, 0));
        const upX = x + c.width - Math.floor(btnW / 2);
        const upY = y + Math.floor(c.height / 4);
        parts.push(
            el(
                "polygon",
                attr("points", `${upX - 4},${upY + 2} ${upX + 4},${upY + 2} ${upX},${upY - 3}`) +
                    attr("fill", "#666666"),
            ),
        );

        // Down button
        parts.push(rect(x + c.width - btnW, y + Math.floor(c.height / 2), btnW, Math.floor(c.height / 2), this._colors.ButtonBackground, border, 1, 0));
        const dnY = y + Math.floor(c.height * 3 / 4);
        parts.push(
            el(
                "polygon",
                attr("points", `${upX - 4},${dnY - 2} ${upX + 4},${dnY - 2} ${upX},${dnY + 3}`) +
                    attr("fill", "#666666"),
            ),
        );
    }

    private renderDateTimePicker(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? this._colors.TextBoxBackground;
        const fg = c.foreground ?? this._colors.TextBoxText;
        const border = c.borderColor ?? this._colors.TextBoxBorder;

        parts.push(rect(x, y, c.width, c.height, bg, border, 1, 2));

        let display = c.dateText;
        if (!display) {
            switch (c.format?.toLowerCase()) {
                case "time":
                    display = "00:00:00";
                    break;
                case "long":
                    display = "2026年1月1日";
                    break;
                default:
                    display = "2026/01/01";
                    break;
            }
        }

        parts.push(txt(display, x + 4, y + Math.floor(c.height / 2), 12, fg));

        // Calendar button
        const btnW = 20;
        parts.push(rect(x + c.width - btnW, y, btnW, c.height, this._colors.ButtonBackground, border, 1, 2));

        // Calendar icon (simplified)
        const ix = x + c.width - Math.floor(btnW / 2);
        const iy = y + Math.floor(c.height / 2);
        parts.push(rect(ix - 5, iy - 5, 10, 10, "none", "#666666", 1, 1));
        parts.push(line(ix - 5, iy - 2, ix + 5, iy - 2, "#666666"));
        parts.push(
            el("circle", attr("cx", ix) + attr("cy", iy + 2) + attr("r", 1) + attr("fill", "#666666")),
        );
    }

    private renderTreeView(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? this._colors.TreeViewBackground;
        const border = c.borderColor ?? this._colors.TextBoxBorder;

        parts.push(rect(x, y, c.width, c.height, bg, border));

        if (c.nodes) {
            let currentY = y + 4;
            for (const node of c.nodes) {
                const raw = node as Partial<TreeNodeDefinition>;
                currentY = this.renderTreeNode(parts, applyTreeNodeDefaults(raw), x + 6, currentY, 0, y + c.height);
            }
        }
    }

    private renderTreeNode(
        parts: string[],
        node: TreeNodeDefinition,
        x: number,
        y: number,
        depth: number,
        maxY: number,
    ): number {
        const nodeH = 20;
        const indent = depth * 16;
        const nx = x + indent;

        if (y + nodeH > maxY) return y;

        // Expand/collapse icon
        if (node.children && node.children.length > 0) {
            if (node.expanded) {
                // ▼
                parts.push(
                    el(
                        "polygon",
                        attr("points", `${nx},${y + 5} ${nx + 8},${y + 5} ${nx + 4},${y + 12}`) +
                            attr("fill", this._colors.TreeViewExpander),
                    ),
                );
            } else {
                // ▶
                parts.push(
                    el(
                        "polygon",
                        attr("points", `${nx},${y + 4} ${nx},${y + 14} ${nx + 7},${y + 9}`) +
                            attr("fill", this._colors.TreeViewExpander),
                    ),
                );
            }
        }

        // Node text
        parts.push(txt(node.text, nx + 14, y + Math.floor(nodeH / 2), 12, this._colors.TreeViewText));

        let nextY = y + nodeH;

        if (node.expanded && node.children) {
            for (const child of node.children) {
                const raw = child as Partial<TreeNodeDefinition>;
                nextY = this.renderTreeNode(parts, applyTreeNodeDefaults(raw), x, nextY, depth + 1, maxY);
                if (nextY + nodeH > maxY) break;
            }
        }

        return nextY;
    }

    private renderToolbar(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? this._colors.ToolbarBackground;
        const fg = c.foreground ?? this._colors.ToolbarText;

        parts.push(rect(x, y, c.width, c.height, bg, "none"));
        parts.push(line(x, y + c.height, x + c.width, y + c.height, this._colors.ToolbarBorder));

        if (c.items) {
            let itemX = x + 4;
            for (const item of c.items) {
                if (item === "|" || item === "-") {
                    // Separator
                    parts.push(line(itemX + 2, y + 4, itemX + 2, y + c.height - 4, this._colors.ToolbarSeparator));
                    itemX += 8;
                } else {
                    const btnW = estimateTextWidth(item, 11) + 12;
                    parts.push(rect(itemX, y + 2, btnW, c.height - 4, "none", "none", 0, 3));
                    parts.push(txt(item, itemX + 6, y + Math.floor(c.height / 2), 11, fg));
                    itemX += btnW + 2;
                }
            }
        }
    }

    private renderLinkLabel(parts: string[], c: ControlDefinition, x: number, y: number): void {
        if (c.text) {
            const fg = c.foreground ?? this._colors.LinkLabelText;
            const textW = estimateTextWidth(c.text, 12);
            parts.push(txt(c.text, x, y + Math.floor(c.height / 2), 12, fg));
            parts.push(
                line(x, y + Math.floor(c.height / 2) + 8, x + textW, y + Math.floor(c.height / 2) + 8, this._colors.LinkLabelUnderline),
            );
        }
    }

    private renderTextArea(parts: string[], c: ControlDefinition, x: number, y: number): void {
        const bg = c.background ?? (c.readOnly ? "#F5F5F5" : this._colors.TextBoxBackground);
        const border = c.borderColor ?? this._colors.TextBoxBorder;

        parts.push(rect(x, y, c.width, c.height, bg, border, 1, 2));

        // Scrollbar
        const scrollW = 14;
        parts.push(rect(x + c.width - scrollW, y + 1, scrollW - 1, c.height - 2, "#F0F0F0", this._colors.GridBorder, 1, 0));
        // Scroll thumb
        parts.push(rect(x + c.width - scrollW + 2, y + 4, scrollW - 5, 20, "#C0C0C0", "none", 0, 3));

        const display = c.text || c.placeholder;
        const fill = c.text ? (c.foreground ?? this._colors.TextBoxText) : this._colors.TextBoxPlaceholder;
        const fontStyle = !c.text && c.placeholder ? "italic" : undefined;

        if (display) {
            const lines = display.split("\n");
            const lineH = 18;
            for (let i = 0; i < lines.length; i++) {
                const ly = y + 12 + i * lineH;
                if (ly > y + c.height - 6) break;
                parts.push(txt(lines[i], x + 4, ly, 12, fill, "start", "normal", fontStyle));
            }
        }
    }

    // ── Annotations ──

    private renderAnnotations(
        parts: string[],
        annotations: AnnotationDefinition[],
        windowX: number,
        windowY: number,
        windowWidth: number,
    ): void {
        const annotationX = windowX + windowWidth + 30;

        const sorted = annotations
            .filter((a) => this._bounds.has(a.target))
            .sort((a, b) => this._bounds.get(a.target)!.centerY - this._bounds.get(b.target)!.centerY);

        // Place with minimum spacing
        const minSpacing = Theme.AnnotationRadius * 2 + 8;
        const positions: Array<{ ann: AnnotationDefinition; circleY: number }> = [];
        let lastY = windowY;

        for (const ann of sorted) {
            const target = this._bounds.get(ann.target)!;
            const desiredY = target.centerY;
            const circleY = Math.max(desiredY, lastY + minSpacing);
            positions.push({ ann, circleY });
            lastY = circleY;
        }

        for (const { ann, circleY } of positions) {
            const target = this._bounds.get(ann.target)!;
            const circleX = annotationX;

            // Leader line
            const lineStartX = target.right + 2;
            const lineStartY = target.centerY;
            const lineEndX = circleX - Theme.AnnotationRadius - 2;

            parts.push(line(lineStartX, lineStartY, lineEndX, circleY, this._colors.AnnotationLine, 1, "4,3"));

            // Connection dot
            parts.push(
                el(
                    "circle",
                    attr("cx", lineStartX) + attr("cy", lineStartY) + attr("r", 3) +
                        attr("fill", this._colors.AnnotationCircle),
                ),
            );

            // Label circle
            const radius = Math.max(Theme.AnnotationRadius, Math.floor(estimateTextWidth(ann.label, 13) / 2) + 4);
            parts.push(
                el(
                    "circle",
                    attr("cx", circleX) + attr("cy", circleY) + attr("r", radius) +
                        attr("fill", this._colors.AnnotationCircle),
                ),
            );

            parts.push(txt(ann.label, circleX, circleY, 13, this._colors.AnnotationText, "middle", "bold"));
        }
    }
}
