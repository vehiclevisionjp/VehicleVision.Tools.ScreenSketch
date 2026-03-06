# YAML 定義リファレンス

YAML ファイルの構造とプロパティの詳細です。

---

## ドキュメント構造

YAML ファイルは以下の 3 つのセクションで構成されます。

```yaml
screen: # 画面メタ情報
window: # ウィンドウ定義（コントロール含む）
annotations: # アノテーション（任意）
```

---

## screen（画面メタ情報）

| プロパティ    | 型                        | 説明                                                                                          |
| ------------- | ------------------------- | --------------------------------------------------------------------------------------------- |
| `title`       | string                    | 画面タイトル（Markdown の見出しに使用）                                                       |
| `description` | string                    | 画面の説明文                                                                                  |
| `theme`       | string                    | カラーテーマ名（`default`, `dark`, `blueprint`, `custom`）。CLI の `--theme` で上書き可       |
| `customTheme` | `map<string, string>`     | カスタムテーマの色定義。`theme: custom` 時に使用し、未指定の要素は標準テーマにフォールバック  |

### customTheme で指定可能なプロパティ

`customTheme` のキーには以下のプロパティ名を指定できます（大文字小文字は区別しません）。値は `#RRGGBB` 形式の16進カラーコードです。

| カテゴリ       | プロパティ名           | 標準テーマの既定値 |
| -------------- | ---------------------- | ------------------ |
| Canvas         | `canvasBackground`     | `#FFFFFF`          |
| Window         | `windowBackground`     | `#F0F0F0`          |
|                | `windowBorder`         | `#999999`          |
|                | `titleBarBackground`   | `#0078D4`          |
|                | `titleBarText`         | `#FFFFFF`          |
| Button         | `buttonBackground`     | `#E1E1E1`          |
|                | `buttonBorder`         | `#ADADAD`          |
|                | `buttonText`           | `#1E1E1E`          |
| TextBox        | `textBoxBackground`    | `#FFFFFF`          |
|                | `textBoxBorder`        | `#7A7A7A`          |
|                | `textBoxText`          | `#1E1E1E`          |
|                | `textBoxPlaceholder`   | `#A0A0A0`          |
| GroupBox       | `groupBorder`          | `#D5DFE5`          |
|                | `groupText`            | `#333333`          |
| DataGrid       | `gridBackground`       | `#FFFFFF`          |
|                | `gridHeaderBackground` | `#F5F5F5`          |
|                | `gridBorder`           | `#D5DFE5`          |
|                | `gridHeaderText`       | `#1E1E1E`          |
|                | `gridCellText`         | `#333333`          |
|                | `gridRowAlternate`     | `#FAFAFA`          |
| MenuBar        | `menuBackground`       | `#F5F5F5`          |
|                | `menuText`             | `#1E1E1E`          |
|                | `menuBorder`           | `#D5DFE5`          |
| StatusBar      | `statusBarBackground`  | `#007ACC`          |
|                | `statusBarText`        | `#FFFFFF`          |
| Label          | `labelText`            | `#1E1E1E`          |
| LinkLabel      | `linkLabelText`        | `#0066CC`          |
|                | `linkLabelUnderline`   | `#0066CC`          |
| TreeView       | `treeViewBackground`   | `#FFFFFF`          |
|                | `treeViewText`         | `#1E1E1E`          |
|                | `treeViewExpander`     | `#666666`          |
| Toolbar        | `toolbarBackground`    | `#F0F0F0`          |
|                | `toolbarBorder`        | `#D5DFE5`          |
|                | `toolbarButtonHover`   | `#E5E5E5`          |
|                | `toolbarText`          | `#1E1E1E`          |
|                | `toolbarSeparator`     | `#C0C0C0`          |
| Annotation     | `annotationCircle`     | `#E53935`          |
|                | `annotationText`       | `#FFFFFF`          |
|                | `annotationLine`       | `#E53935`          |
| Chromeless     | `chromelessBorder`     | `#D0D0D0`          |

**使用例：**

```yaml
screen:
  title: カスタムテーマ
  theme: custom
  customTheme:
    canvasBackground: "#F5F0EB"
    windowBackground: "#FAF7F2"
    windowBorder: "#8B7355"
    titleBarBackground: "#6B4226"
    titleBarText: "#FFFFFF"
    buttonBackground: "#D2B48C"
    labelText: "#3E2723"
```

---

## window（ウィンドウ定義）

| プロパティ | 型                  | デフォルト | 説明                                                                                 |
| ---------- | ------------------- | ---------- | ------------------------------------------------------------------------------------ |
| `title`    | string              | `""`       | タイトルバーに表示するテキスト                                                       |
| `width`    | int                 | `800`      | ウィンドウ幅（px）                                                                   |
| `height`   | int                 | `600`      | ウィンドウ高さ（px）                                                                 |
| `chrome`   | bool                | `true`     | `false` でタイトルバー・枠線・影を省略し、コンテンツ領域のみを描画（部分描画モード） |
| `controls` | ControlDefinition[] |            | 子コントロールのリスト                                                               |

---

## annotations（アノテーション）

コントロールにラベル（①②③ など）を付け、説明をテーブルとして出力します。

| プロパティ    | 型     | 説明                       |
| ------------- | ------ | -------------------------- |
| `target`      | string | 対象コントロールの `id`    |
| `label`       | string | 表示ラベル（例: `"①"`）    |
| `description` | string | マニュアルに記載する説明文 |
