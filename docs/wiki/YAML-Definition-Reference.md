# YAML 定義リファレンス

YAML ファイルの構造とプロパティの詳細です。

---

## ドキュメント構造

YAML ファイルは以下の 4 つのセクションで構成されます。

```yaml
screen: # 画面メタ情報
window: # ウィンドウ定義（コントロール含む）
annotations: # アノテーション（任意）
connectors: # コネクタ線（任意）
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

`customTheme` のキーには以下のプロパティ名を指定できます（大文字小文字は区別しません）。値は `#RRGGBB` 形式の16進カラーコード、または WinForms の名前付き色（例: `Control`, `ActiveBorder`, `Window`）を指定できます。

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
| Connector      | `connectorLine`        | `#1976D2`          |
|                | `connectorCircle`      | `#1976D2`          |
|                | `connectorText`        | `#FFFFFF`          |
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

| プロパティ        | 型     | 説明                                                        |
| ----------------- | ------ | ----------------------------------------------------------- |
| `target`          | string | 対象コントロールの `id`                                     |
| `label`           | string | 表示ラベル（例: `"①"`）                                     |
| `description`     | string | マニュアルに記載する説明文                                  |
| `lineColor`       | string | 引き出し線の色オーバーライド（例: `"#1976D2"`）             |
| `lineStyle`       | string | 引き出し線のスタイル（`solid`, `dashed`, `dotted`）省略時は `dashed` |
| `labelBackground` | string | ラベル円の背景色オーバーライド（例: `"#4CAF50"`）           |
| `labelColor`      | string | ラベルテキストの色オーバーライド（例: `"#FFFFFF"`）         |

---

## connectors（コネクタ線）

コントロール間を結ぶ線を描画し、手順や操作フローを図示します。接続先に矢印が付き、中間点にラベルが表示されます。

| プロパティ        | 型     | 説明                                                        |
| ----------------- | ------ | ----------------------------------------------------------- |
| `from`            | string | 接続元コントロールの `id`                                   |
| `to`              | string | 接続先コントロールの `id`                                   |
| `label`           | string | 表示ラベル（例: `"①"`）                                     |
| `description`     | string | マニュアルに記載する説明文                                  |
| `lineColor`       | string | コネクタ線の色オーバーライド（例: `"#4CAF50"` または `SteelBlue`） |
| `lineStyle`       | string | コネクタ線のスタイル（`solid`, `dashed`, `dotted`）省略時は `solid` |
| `labelBackground` | string | ラベル円の背景色オーバーライド（例: `"#FF9800"`）           |
| `labelColor`      | string | ラベルテキストの色オーバーライド（例: `"#FFFFFF"`）         |
| `fromShape`       | string | 接続元の端点形状（`none`, `arrow`, `circle`, `diamond`, `square`）省略時は `none` |
| `toShape`         | string | 接続先の端点形状（`none`, `arrow`, `circle`, `diamond`, `square`）省略時は `arrow` |
| `fromAnchor`      | string | 接続元のアンカー位置（`auto`, `top`, `bottom`, `left`, `right`, `center`）省略時は `auto` |
| `toAnchor`        | string | 接続先のアンカー位置（`auto`, `top`, `bottom`, `left`, `right`, `center`）省略時は `auto` |
| `lineType`        | string | 線の種類（`straight`, `curve`）省略時は `straight`          |

### 端点形状（fromShape / toShape）

| 値        | 形状         |
| --------- | ------------ |
| `none`    | なし         |
| `arrow`   | 三角矢印     |
| `circle`  | 小円         |
| `diamond` | ひし形       |
| `square`  | 四角形       |

### アンカー位置（fromAnchor / toAnchor）

| 値       | 説明                                     |
| -------- | ---------------------------------------- |
| `auto`   | 相手コントロールへの最短エッジ点（既定） |
| `top`    | コントロール上辺の中央                   |
| `bottom` | コントロール下辺の中央                   |
| `left`   | コントロール左辺の中央                   |
| `right`  | コントロール右辺の中央                   |
| `center` | コントロール中心                         |

### 線の種類（lineType）

| 値         | 説明                                         |
| ---------- | -------------------------------------------- |
| `straight` | 直線（既定）                                 |
| `curve`    | ベジェ曲線（アンカー方向に沿って滑らかに曲がる） |

**使用例：**

```yaml
connectors:
  - from: nameInput
    to: emailInput
    label: "①"
    description: 名前を入力後、メールアドレスを入力します。
    fromAnchor: bottom
    toAnchor: top
    fromShape: circle
    toShape: arrow
  - from: emailInput
    to: submitBtn
    label: "②"
    description: 登録ボタンを押します。
    lineType: curve
    fromAnchor: bottom
    toAnchor: top
    lineColor: "#4CAF50"
    labelBackground: "#4CAF50"
```

---

## 色の指定

色プロパティ（`background`, `foreground`, `borderColor`, `lineColor`, `labelBackground`, `labelColor` など）には以下の形式が使用できます。

| 形式                | 例                        | 説明                                                     |
| ------------------- | ------------------------- | -------------------------------------------------------- |
| 16進カラーコード    | `"#FF0000"`, `"#1976D2"`  | `#RRGGBB` 形式                                          |
| WinForms 名前付き色 | `Control`, `ActiveBorder` | `System.Drawing.KnownColor` のラベル名（大文字小文字不問） |

### よく使う名前付き色の例

| 名前             | 用途                   | 色       |
| ---------------- | ---------------------- | -------- |
| `Control`        | コントロール背景       | `#ECE9D8` |
| `ControlText`    | コントロール文字色     | `#000000` |
| `ControlDark`    | コントロール暗い枠線   | `#ACA899` |
| `Window`         | ウィンドウ背景         | `#FFFFFF` |
| `WindowText`     | ウィンドウ文字色       | `#000000` |
| `WindowFrame`    | ウィンドウ枠線         | `#000000` |
| `Highlight`      | 選択背景               | `#316AC5` |
| `HighlightText`  | 選択文字色             | `#FFFFFF` |
| `ActiveBorder`   | アクティブ枠線         | `#D4D0C8` |
| `ButtonFace`     | ボタン背景             | `#ECE9D8` |
| `GrayText`       | 無効テキスト           | `#ACA899` |
| `SteelBlue`      | Web カラー（青灰色）   | `#4682B4` |
| `Tomato`         | Web カラー（トマト色） | `#FF6347` |

> **注意:** システムカラー（`Control`, `ActiveBorder` 等）の実際の RGB 値は実行環境の OS テーマに依存します。上記の値は Windows クラシックテーマの既定値です。`Transparent` 等の透明度を持つ色は `#RRGGBB` 形式（不透明）に変換されます。
