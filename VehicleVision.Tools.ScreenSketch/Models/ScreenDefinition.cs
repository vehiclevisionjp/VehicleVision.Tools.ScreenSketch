namespace VehicleVision.Tools.ScreenSketch.Models;

/// <summary>YAML ルートドキュメント</summary>
public class ScreenDefinition
{
    public ScreenInfo? Screen { get; set; }
    public WindowDefinition? Window { get; set; }
    public List<AnnotationDefinition>? Annotations { get; set; }
}

/// <summary>画面メタ情報</summary>
public class ScreenInfo
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";

    /// <summary>カラーテーマ名（default, dark, blueprint）</summary>
    public string? Theme { get; set; }
}

/// <summary>ウィンドウ定義</summary>
public class WindowDefinition
{
    public string Title { get; set; } = "";
    public int Width { get; set; } = 800;
    public int Height { get; set; } = 600;

    /// <summary>ウィンドウ装飾（タイトルバー・枠線・影）の表示。false でコンテンツ領域のみ描画</summary>
    public bool Chrome { get; set; } = true;

    public List<ControlDefinition>? Controls { get; set; }
}

/// <summary>コントロール定義（全コントロール共通）</summary>
public class ControlDefinition
{
    /// <summary>
    /// コントロール種別:
    /// button, textbox, label, combobox, checkbox, radiobutton,
    /// group, datagrid, menubar, statusbar, tabcontrol, listbox,
    /// panel, image, progressbar, numericupdown, datetimepicker,
    /// treeview, toolbar, linklabel, textarea
    /// </summary>
    public string Type { get; set; } = "";

    /// <summary>アノテーション対象として参照する ID</summary>
    public string? Id { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    /// <summary>表示テキスト（ボタン、ラベル、テキストボックスなど）</summary>
    public string? Text { get; set; }

    /// <summary>プレースホルダー（テキストボックス）</summary>
    public string? Placeholder { get; set; }

    /// <summary>チェック状態（チェックボックス）</summary>
    public bool Checked { get; set; }

    /// <summary>選択状態（ラジオボタン）</summary>
    public bool Selected { get; set; }

    /// <summary>選択中インデックス（リストボックス）</summary>
    public int SelectedIndex { get; set; } = -1;

    /// <summary>値（プログレスバー: 0-100）</summary>
    public int Value { get; set; }

    /// <summary>子コントロール（group, panel）</summary>
    public List<ControlDefinition>? Controls { get; set; }

    /// <summary>列定義（datagrid）</summary>
    public List<ColumnDefinition>? Columns { get; set; }

    /// <summary>行データ（datagrid）</summary>
    public List<List<string>>? Rows { get; set; }

    /// <summary>文字列アイテム（menubar, statusbar, listbox, combobox）</summary>
    public List<string>? Items { get; set; }

    /// <summary>タブ定義（tabcontrol）</summary>
    public List<TabDefinition>? Tabs { get; set; }

    /// <summary>アクティブなタブのインデックス（tabcontrol）</summary>
    public int ActiveTab { get; set; }

    /// <summary>最小値（numericupdown）</summary>
    public int Minimum { get; set; }

    /// <summary>最大値（numericupdown）</summary>
    public int Maximum { get; set; } = 100;

    /// <summary>日付表示テキスト（datetimepicker）</summary>
    public string? DateText { get; set; }

    /// <summary>日付フォーマット（datetimepicker: Short, Long, Time, Custom）</summary>
    public string? Format { get; set; }

    /// <summary>ツリーノード（treeview）</summary>
    public List<TreeNodeDefinition>? Nodes { get; set; }

    /// <summary>リンク先 URL（linklabel）</summary>
    public string? Url { get; set; }

    /// <summary>読み取り専用（textarea）</summary>
    public bool ReadOnly { get; set; }

    /// <summary>背景色のオーバーライド（例: "#FF0000"）</summary>
    public string? Background { get; set; }

    /// <summary>前景色（テキスト色）のオーバーライド（例: "#FFFFFF"）</summary>
    public string? Foreground { get; set; }

    /// <summary>枠線色のオーバーライド（例: "#0000FF"）</summary>
    public string? BorderColor { get; set; }
}

/// <summary>DataGrid 列定義</summary>
public class ColumnDefinition
{
    public string Header { get; set; } = "";
    public int Width { get; set; } = 100;
}

/// <summary>TabControl タブ定義</summary>
public class TabDefinition
{
    public string Text { get; set; } = "";
    public List<ControlDefinition>? Controls { get; set; }
}

/// <summary>TreeView ノード定義</summary>
public class TreeNodeDefinition
{
    public string Text { get; set; } = "";
    public bool Expanded { get; set; } = true;
    public List<TreeNodeDefinition>? Children { get; set; }
}

/// <summary>アノテーション（ラベル付け）定義</summary>
public class AnnotationDefinition
{
    /// <summary>対象コントロールの Id</summary>
    public string Target { get; set; } = "";

    /// <summary>表示ラベル（①②③ など）</summary>
    public string Label { get; set; } = "";

    /// <summary>マニュアルに記載する説明文</summary>
    public string Description { get; set; } = "";
}
