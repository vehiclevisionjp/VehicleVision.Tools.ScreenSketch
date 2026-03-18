# コントロール一覧

VehicleVision.Tools.ScreenSketch が対応するコントロールの一覧です。

## 対応コントロール

| コントロール | type値 | 説明 |
| ------------ | ------ | ---- |
| ボタン | `button` | クリック操作用ボタン |
| テキストボックス | `textbox` | 単行テキスト入力 |
| ラベル | `label` | テキスト表示 |
| コンボボックス | `combobox` | ドロップダウン選択 |
| チェックボックス | `checkbox` | オン/オフ切り替え |
| ラジオボタン | `radiobutton` | 排他選択 |
| グループ | `group` | コントロールのグループ化 |
| データグリッド | `datagrid` | 表形式データ表示 |
| メニューバー | `menubar` | メニュー表示 |
| ステータスバー | `statusbar` | ステータス表示 |
| タブコントロール | `tabcontrol` | タブ切り替え |
| リストボックス | `listbox` | リスト選択 |
| パネル | `panel` | コントロールのコンテナ |
| 画像 | `image` | 画像表示 |
| プログレスバー | `progressbar` | 進捗表示 |
| 数値入力 | `numericupdown` | 数値入力 |
| 日時選択 | `datetimepicker` | 日付・時刻選択 |
| ツリービュー | `treeview` | ツリー構造表示 |
| ツールバー | `toolbar` | ツールボタン表示 |
| リンクラベル | `linklabel` | リンク付きテキスト |
| テキストエリア | `textarea` | 複数行テキスト入力 |

## 定義例

```yaml
controls:
  - type: button
    text: 検索
    x: 200
    y: 50
    width: 80
    height: 30
  - type: textbox
    text: ""
    x: 10
    y: 50
    width: 180
    height: 25
```

## 関連ページ

- [YAML定義リファレンス](YAML-Definition-Reference)
- [サンプル集](Examples)
- [Home](Home)
