// Theme dimension / typography constants
export const Theme = {
    FontFamily: "'Segoe UI', 'Meiryo', sans-serif",
    DefaultFontSize: 12,
    TitleBarHeight: 30,
    MenuBarHeight: 22,
    StatusBarHeight: 22,
    AnnotationMargin: 120,
    AnnotationRadius: 14,
    WindowPadding: 40,
    ChromelessPadding: 10,
} as const;

export interface ThemeColorValues {
    windowBackground: string;
    windowBorder: string;
    titleBarBackground: string;
    titleBarText: string;
    buttonBackground: string;
    buttonBorder: string;
    buttonText: string;
    textBoxBackground: string;
    textBoxBorder: string;
    textBoxText: string;
    textBoxPlaceholder: string;
    groupBorder: string;
    groupText: string;
    gridBackground: string;
    gridHeaderBackground: string;
    gridBorder: string;
    gridHeaderText: string;
    gridCellText: string;
    gridRowAlternate: string;
    menuBackground: string;
    menuText: string;
    menuBorder: string;
    statusBarBackground: string;
    statusBarText: string;
    labelText: string;
    linkLabelText: string;
    linkLabelUnderline: string;
    treeViewBackground: string;
    treeViewText: string;
    treeViewExpander: string;
    toolbarBackground: string;
    toolbarBorder: string;
    toolbarButtonHover: string;
    toolbarText: string;
    toolbarSeparator: string;
    annotationCircle: string;
    annotationText: string;
    annotationLine: string;
    chromelessBorder: string;
    canvasBackground: string;
}

const defaultColors: ThemeColorValues = {
    windowBackground: "#F0F0F0",
    windowBorder: "#999999",
    titleBarBackground: "#0078D4",
    titleBarText: "#FFFFFF",
    buttonBackground: "#E1E1E1",
    buttonBorder: "#ADADAD",
    buttonText: "#1E1E1E",
    textBoxBackground: "#FFFFFF",
    textBoxBorder: "#7A7A7A",
    textBoxText: "#1E1E1E",
    textBoxPlaceholder: "#A0A0A0",
    groupBorder: "#D5DFE5",
    groupText: "#333333",
    gridBackground: "#FFFFFF",
    gridHeaderBackground: "#F5F5F5",
    gridBorder: "#D5DFE5",
    gridHeaderText: "#1E1E1E",
    gridCellText: "#333333",
    gridRowAlternate: "#FAFAFA",
    menuBackground: "#F5F5F5",
    menuText: "#1E1E1E",
    menuBorder: "#D5DFE5",
    statusBarBackground: "#007ACC",
    statusBarText: "#FFFFFF",
    labelText: "#1E1E1E",
    linkLabelText: "#0066CC",
    linkLabelUnderline: "#0066CC",
    treeViewBackground: "#FFFFFF",
    treeViewText: "#1E1E1E",
    treeViewExpander: "#666666",
    toolbarBackground: "#F0F0F0",
    toolbarBorder: "#D5DFE5",
    toolbarButtonHover: "#E5E5E5",
    toolbarText: "#1E1E1E",
    toolbarSeparator: "#C0C0C0",
    annotationCircle: "#E53935",
    annotationText: "#FFFFFF",
    annotationLine: "#E53935",
    chromelessBorder: "#D0D0D0",
    canvasBackground: "#FFFFFF",
};

const darkColors: ThemeColorValues = {
    canvasBackground: "#1E1E1E",
    windowBackground: "#2D2D2D",
    windowBorder: "#555555",
    titleBarBackground: "#3C3C3C",
    titleBarText: "#CCCCCC",
    buttonBackground: "#3C3C3C",
    buttonBorder: "#555555",
    buttonText: "#CCCCCC",
    textBoxBackground: "#3C3C3C",
    textBoxBorder: "#555555",
    textBoxText: "#CCCCCC",
    textBoxPlaceholder: "#808080",
    groupBorder: "#555555",
    groupText: "#CCCCCC",
    gridBackground: "#2D2D2D",
    gridHeaderBackground: "#383838",
    gridBorder: "#555555",
    gridHeaderText: "#CCCCCC",
    gridCellText: "#BBBBBB",
    gridRowAlternate: "#333333",
    menuBackground: "#2D2D2D",
    menuText: "#CCCCCC",
    menuBorder: "#555555",
    statusBarBackground: "#007ACC",
    statusBarText: "#FFFFFF",
    labelText: "#CCCCCC",
    linkLabelText: "#4FC1FF",
    linkLabelUnderline: "#4FC1FF",
    treeViewBackground: "#2D2D2D",
    treeViewText: "#CCCCCC",
    treeViewExpander: "#AAAAAA",
    toolbarBackground: "#2D2D2D",
    toolbarBorder: "#555555",
    toolbarButtonHover: "#454545",
    toolbarText: "#CCCCCC",
    toolbarSeparator: "#555555",
    annotationCircle: "#E53935",
    annotationText: "#FFFFFF",
    annotationLine: "#E53935",
    chromelessBorder: "#555555",
};

const blueprintColors: ThemeColorValues = {
    canvasBackground: "#1A3A5C",
    windowBackground: "#1A3A5C",
    windowBorder: "#4A90D9",
    titleBarBackground: "#2E6EB5",
    titleBarText: "#FFFFFF",
    buttonBackground: "#1A3A5C",
    buttonBorder: "#4A90D9",
    buttonText: "#FFFFFF",
    textBoxBackground: "#0F2A45",
    textBoxBorder: "#4A90D9",
    textBoxText: "#FFFFFF",
    textBoxPlaceholder: "#7BAAD4",
    groupBorder: "#4A90D9",
    groupText: "#C0DEFF",
    gridBackground: "#0F2A45",
    gridHeaderBackground: "#1A3A5C",
    gridBorder: "#4A90D9",
    gridHeaderText: "#FFFFFF",
    gridCellText: "#C0DEFF",
    gridRowAlternate: "#15304F",
    menuBackground: "#1A3A5C",
    menuText: "#FFFFFF",
    menuBorder: "#4A90D9",
    statusBarBackground: "#2E6EB5",
    statusBarText: "#FFFFFF",
    labelText: "#FFFFFF",
    linkLabelText: "#7EC8E3",
    linkLabelUnderline: "#7EC8E3",
    treeViewBackground: "#0F2A45",
    treeViewText: "#FFFFFF",
    treeViewExpander: "#7BAAD4",
    toolbarBackground: "#1A3A5C",
    toolbarBorder: "#4A90D9",
    toolbarButtonHover: "#2E6EB5",
    toolbarText: "#FFFFFF",
    toolbarSeparator: "#4A90D9",
    annotationCircle: "#FF6B35",
    annotationText: "#FFFFFF",
    annotationLine: "#FF6B35",
    chromelessBorder: "#4A90D9",
};

const HEX_COLOR_RE = /^#[0-9A-Fa-f]{6}$/;

function createCustom(overrides: Record<string, string> | undefined): ThemeColorValues {
    const colors: ThemeColorValues = { ...defaultColors };
    if (!overrides) return colors;

    // Build case-insensitive property map
    const keyMap = new Map<string, keyof ThemeColorValues>();
    for (const k of Object.keys(colors) as (keyof ThemeColorValues)[]) {
        keyMap.set(k.toLowerCase(), k);
    }

    for (const [key, value] of Object.entries(overrides)) {
        const prop = keyMap.get(key.toLowerCase());
        if (prop && HEX_COLOR_RE.test(value)) {
            (colors as unknown as Record<string, string>)[prop] = value;
        }
    }

    return colors;
}

export class ThemeColors {
    private readonly _c: ThemeColorValues;

    constructor(colors?: ThemeColorValues) {
        this._c = colors ?? { ...defaultColors };
    }

    // ── accessors (match C# property names used in SvgRenderer) ──
    get WindowBackground() { return this._c.windowBackground; }
    get WindowBorder() { return this._c.windowBorder; }
    get TitleBarBackground() { return this._c.titleBarBackground; }
    get TitleBarText() { return this._c.titleBarText; }
    get ButtonBackground() { return this._c.buttonBackground; }
    get ButtonBorder() { return this._c.buttonBorder; }
    get ButtonText() { return this._c.buttonText; }
    get TextBoxBackground() { return this._c.textBoxBackground; }
    get TextBoxBorder() { return this._c.textBoxBorder; }
    get TextBoxText() { return this._c.textBoxText; }
    get TextBoxPlaceholder() { return this._c.textBoxPlaceholder; }
    get GroupBorder() { return this._c.groupBorder; }
    get GroupText() { return this._c.groupText; }
    get GridBackground() { return this._c.gridBackground; }
    get GridHeaderBackground() { return this._c.gridHeaderBackground; }
    get GridBorder() { return this._c.gridBorder; }
    get GridHeaderText() { return this._c.gridHeaderText; }
    get GridCellText() { return this._c.gridCellText; }
    get GridRowAlternate() { return this._c.gridRowAlternate; }
    get MenuBackground() { return this._c.menuBackground; }
    get MenuText() { return this._c.menuText; }
    get MenuBorder() { return this._c.menuBorder; }
    get StatusBarBackground() { return this._c.statusBarBackground; }
    get StatusBarText() { return this._c.statusBarText; }
    get LabelText() { return this._c.labelText; }
    get LinkLabelText() { return this._c.linkLabelText; }
    get LinkLabelUnderline() { return this._c.linkLabelUnderline; }
    get TreeViewBackground() { return this._c.treeViewBackground; }
    get TreeViewText() { return this._c.treeViewText; }
    get TreeViewExpander() { return this._c.treeViewExpander; }
    get ToolbarBackground() { return this._c.toolbarBackground; }
    get ToolbarBorder() { return this._c.toolbarBorder; }
    get ToolbarButtonHover() { return this._c.toolbarButtonHover; }
    get ToolbarText() { return this._c.toolbarText; }
    get ToolbarSeparator() { return this._c.toolbarSeparator; }
    get AnnotationCircle() { return this._c.annotationCircle; }
    get AnnotationText() { return this._c.annotationText; }
    get AnnotationLine() { return this._c.annotationLine; }
    get ChromelessBorder() { return this._c.chromelessBorder; }
    get CanvasBackground() { return this._c.canvasBackground; }

    static fromName(
        themeName: string | undefined,
        customOverrides?: Record<string, string>,
    ): ThemeColors {
        switch (themeName?.toLowerCase()) {
            case "dark":
                return new ThemeColors(darkColors);
            case "blueprint":
                return new ThemeColors(blueprintColors);
            case "custom":
                return new ThemeColors(createCustom(customOverrides));
            default:
                return new ThemeColors();
        }
    }
}
