export interface ScreenDefinition {
    screen?: ScreenInfo;
    window?: WindowDefinition;
    annotations?: AnnotationDefinition[];
}

export interface ScreenInfo {
    title?: string;
    description?: string;
    theme?: string;
    customTheme?: Record<string, string>;
}

export interface WindowDefinition {
    title?: string;
    width: number;
    height: number;
    chrome: boolean;
    controls?: ControlDefinition[];
}

export interface ControlDefinition {
    type: string;
    id?: string;
    x: number;
    y: number;
    width: number;
    height: number;
    text?: string;
    placeholder?: string;
    checked?: boolean;
    selected?: boolean;
    selectedIndex: number;
    value: number;
    controls?: ControlDefinition[];
    columns?: ColumnDefinition[];
    rows?: string[][];
    items?: string[];
    tabs?: TabDefinition[];
    activeTab: number;
    minimum: number;
    maximum: number;
    dateText?: string;
    format?: string;
    nodes?: TreeNodeDefinition[];
    url?: string;
    readOnly?: boolean;
    background?: string;
    foreground?: string;
    borderColor?: string;
}

export interface ColumnDefinition {
    header: string;
    width: number;
}

export interface TabDefinition {
    text: string;
    controls?: ControlDefinition[];
}

export interface TreeNodeDefinition {
    text: string;
    expanded: boolean;
    children?: TreeNodeDefinition[];
}

export interface AnnotationDefinition {
    target: string;
    label: string;
    description?: string;
}

/** Apply default values to a raw YAML-parsed WindowDefinition. */
export function applyWindowDefaults(raw: Partial<WindowDefinition> | undefined): WindowDefinition {
    return {
        width: 800,
        height: 600,
        chrome: true,
        ...raw,
    };
}

/** Apply default values to a raw YAML-parsed ControlDefinition. */
export function applyControlDefaults(raw: Record<string, unknown>): ControlDefinition {
    return {
        type: "",
        x: 0,
        y: 0,
        width: 0,
        height: 0,
        value: 0,
        selectedIndex: -1,
        activeTab: 0,
        minimum: 0,
        maximum: 100,
        ...raw,
    } as ControlDefinition;
}

/** Apply default values to a raw YAML-parsed ColumnDefinition. */
export function applyColumnDefaults(raw: Partial<ColumnDefinition>): ColumnDefinition {
    return {
        header: "",
        width: 100,
        ...raw,
    };
}

/** Apply default values to a raw YAML-parsed TreeNodeDefinition. */
export function applyTreeNodeDefaults(raw: Partial<TreeNodeDefinition>): TreeNodeDefinition {
    return {
        text: "",
        expanded: true,
        ...raw,
    };
}
