# 対応コントロール

## コントロール共通プロパティ

| プロパティ | 型     | 説明                                       |
| ---------- | ------ | ------------------------------------------ |
| `type`     | string | コントロール種別（必須）                   |
| `id`       | string | アノテーション用の識別子（任意）           |
| `x`        | int    | X 座標（親コンテナからの相対位置）         |
| `y`        | int    | Y 座標（親コンテナからの相対位置）         |
| `width`    | int    | 幅（コントロールごとにデフォルト値あり）   |
| `height`   | int    | 高さ（コントロールごとにデフォルト値あり） |

---

## コントロール一覧

---

### button（ボタン）

![button](../../images/button.svg)

| プロパティ | 型     | 説明         |
| ---------- | ------ | ------------ |
| `text`     | string | 表示テキスト |

デフォルトサイズ: 80 × 26

---

### textbox（テキストボックス）

![textbox](../../images/textbox.svg)

| プロパティ    | 型     | 説明             |
| ------------- | ------ | ---------------- |
| `text`        | string | 入力テキスト     |
| `placeholder` | string | プレースホルダー |

デフォルトサイズ: 150 × 24

---

### textarea（テキストエリア）

![textarea](../../images/textarea.svg)

| プロパティ    | 型     | 説明             |
| ------------- | ------ | ---------------- |
| `text`        | string | 入力テキスト     |
| `placeholder` | string | プレースホルダー |
| `readOnly`    | bool   | 読み取り専用     |

デフォルトサイズ: 200 × 80

---

### label（ラベル）

![label](../../images/label.svg)

| プロパティ | 型     | 説明         |
| ---------- | ------ | ------------ |
| `text`     | string | 表示テキスト |

デフォルトサイズ: 自動 × 20

---

### linklabel（リンクラベル）

![linklabel](../../images/linklabel.svg)

| プロパティ | 型     | 説明         |
| ---------- | ------ | ------------ |
| `text`     | string | 表示テキスト |
| `url`      | string | リンク先 URL |

デフォルトサイズ: 自動 × 20

---

### combobox（コンボボックス）

![combobox](../../images/combobox.svg)

| プロパティ | 型       | 説明           |
| ---------- | -------- | -------------- |
| `text`     | string   | 選択中テキスト |
| `items`    | string[] | 選択肢リスト   |

デフォルトサイズ: 150 × 24

---

### checkbox（チェックボックス）

![checkbox](../../images/checkbox.svg)

| プロパティ | 型     | 説明         |
| ---------- | ------ | ------------ |
| `text`     | string | 表示テキスト |
| `checked`  | bool   | チェック状態 |

デフォルトサイズ: 自動 × 20

---

### radiobutton（ラジオボタン）

![radiobutton](../../images/radiobutton.svg)

| プロパティ | 型     | 説明         |
| ---------- | ------ | ------------ |
| `text`     | string | 表示テキスト |
| `selected` | bool   | 選択状態     |

デフォルトサイズ: 自動 × 20

---

### group（グループ）

![group](../../images/group.svg)

| プロパティ | 型                  | 説明                 |
| ---------- | ------------------- | -------------------- |
| `text`     | string              | グループラベル       |
| `controls` | ControlDefinition[] | 子コントロールリスト |

デフォルトサイズ: 指定必須

```yaml
- type: group
  text: '設定'
  x: 10
  y: 10
  width: 300
  height: 100
  controls:
      - type: checkbox
        text: 'オプションA'
        x: 15
        y: 15
        checked: true
```

---

### datagrid（データグリッド）

![datagrid](../../images/datagrid.svg)

| プロパティ | 型                 | 説明     |
| ---------- | ------------------ | -------- |
| `columns`  | ColumnDefinition[] | 列定義   |
| `rows`     | string[][]         | 行データ |

デフォルトサイズ: 指定必須

```yaml
- type: datagrid
  x: 10
  y: 10
  width: 400
  height: 200
  columns:
      - header: '名前'
        width: 150
      - header: '値'
        width: 250
  rows:
      - ['項目1', '値1']
      - ['項目2', '値2']
```

---

### menubar（メニューバー）

![menubar](../../images/menubar.svg)

| プロパティ | 型       | 説明         |
| ---------- | -------- | ------------ |
| `items`    | string[] | メニュー項目 |

デフォルトサイズ: コンテナ幅 × 22

---

### statusbar（ステータスバー）

![statusbar](../../images/statusbar.svg)

| プロパティ | 型       | 説明               |
| ---------- | -------- | ------------------ |
| `items`    | string[] | ステータスバー項目 |

デフォルトサイズ: コンテナ幅 × 22

---

### tabcontrol（タブコントロール）

![tabcontrol](../../images/tabcontrol.svg)

| プロパティ  | 型              | 説明               |
| ----------- | --------------- | ------------------ |
| `tabs`      | TabDefinition[] | タブ定義           |
| `activeTab` | int             | アクティブタブ番号 |

デフォルトサイズ: 指定必須

```yaml
- type: tabcontrol
  x: 10
  y: 10
  width: 400
  height: 300
  activeTab: 0
  tabs:
      - text: 'タブ1'
        controls:
            - type: label
              text: 'タブ1の内容'
              x: 10
              y: 10
      - text: 'タブ2'
        controls: []
```

---

### listbox（リストボックス）

![listbox](../../images/listbox.svg)

| プロパティ      | 型       | 説明               |
| --------------- | -------- | ------------------ |
| `items`         | string[] | 項目リスト         |
| `selectedIndex` | int      | 選択中インデックス |

デフォルトサイズ: 指定必須

---

### panel（パネル）

![panel](../../images/panel.svg)

| プロパティ | 型                  | 説明                 |
| ---------- | ------------------- | -------------------- |
| `controls` | ControlDefinition[] | 子コントロールリスト |

デフォルトサイズ: 指定必須

---

### image（画像プレースホルダ）

![image](../../images/image.svg)

| プロパティ | 型     | 説明         |
| ---------- | ------ | ------------ |
| `text`     | string | 代替テキスト |

デフォルトサイズ: 指定必須

---

### progressbar（プログレスバー）

![progressbar](../../images/progressbar.svg)

| プロパティ | 型  | 説明            |
| ---------- | --- | --------------- |
| `value`    | int | 進捗値（0-100） |

デフォルトサイズ: 200 × 20

---

### numericupdown（数値入力）

![numericupdown](../../images/numericupdown.svg)

| プロパティ | 型  | 説明   |
| ---------- | --- | ------ |
| `value`    | int | 現在値 |
| `minimum`  | int | 最小値 |
| `maximum`  | int | 最大値 |

デフォルトサイズ: 100 × 24

---

### datetimepicker（日付時刻入力）

![datetimepicker](../../images/datetimepicker.svg)

| プロパティ | 型     | 説明                                      |
| ---------- | ------ | ----------------------------------------- |
| `dateText` | string | 表示する日付テキスト                      |
| `format`   | string | フォーマット（`short` / `long` / `time`） |

デフォルトサイズ: 180 × 24

---

### treeview（ツリービュー）

![treeview](../../images/treeview.svg)

| プロパティ | 型                   | 説明       |
| ---------- | -------------------- | ---------- |
| `nodes`    | TreeNodeDefinition[] | ノード定義 |

デフォルトサイズ: 指定必須

```yaml
- type: treeview
  x: 10
  y: 10
  width: 250
  height: 200
  nodes:
      - text: 'フォルダ1'
        expanded: true
        children:
            - text: 'ファイル1.txt'
            - text: 'ファイル2.txt'
      - text: 'フォルダ2'
        expanded: false
        children:
            - text: 'ファイル3.txt'
```

---

### toolbar（ツールバー）

![toolbar](../../images/toolbar.svg)

| プロパティ | 型       | 説明                                       |
| ---------- | -------- | ------------------------------------------ |
| `items`    | string[] | ボタン項目（`"\|"` or `"-"` でセパレータ） |

デフォルトサイズ: コンテナ幅 × 28
