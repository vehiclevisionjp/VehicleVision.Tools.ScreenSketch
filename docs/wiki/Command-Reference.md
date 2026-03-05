# コマンドリファレンス

VehicleVision.Tools.ScreenSketch のコマンド一覧です。

---

## generate

YAML ファイルから SVG 画面イメージと Markdown ドキュメントを生成します。

```bash
screen-sketch generate <input-path> [output-dir] [--theme <name>]
```

| 引数           | 必須 | 説明                                                |
| -------------- | ---- | --------------------------------------------------- |
| `<input-path>` | ○    | YAML ファイルまたは YAML ファイルを含むディレクトリ |
| `[output-dir]` |      | 出力先ディレクトリ（省略時: `./output`）            |
| `--theme`      |      | カラーテーマ（`default`, `dark`, `blueprint`）   |

**出力ファイル：**

- `<output-dir>/images/<name>.svg` — 画面イメージ（SVG）
- `<output-dir>/<name>.md` — 画面ドキュメント（Markdown）
- `<output-dir>/index.md` — 目次ページ（複数ファイル時のみ）

---

## transform

Markdown ファイル内の ` ```yaml-screen ` コードブロックを検出し、SVG 画像参照 + アノテーションテーブルに変換します。元の YAML は HTML コメントとして保持されるため、再変換や復元が可能です。

```bash
screen-sketch transform <input-path> [output-dir] [--inline] [--theme <name>]
```

| 引数           | 必須 | 説明                                                      |
| -------------- | ---- | --------------------------------------------------------- |
| `<input-path>` | ○    | Markdown ファイルまたはディレクトリ                       |
| `[output-dir]` |      | 出力先ディレクトリ（省略時: 入力と同じ場所に上書き）      |
| `--inline`     |      | SVG をインライン埋め込みする（PDF/HTML 生成の前処理向け） |
| `--theme`      |      | カラーテーマ（`default`, `dark`, `blueprint`）           |

**変換前（Markdown）：**

````markdown
```yaml-screen
screen:
  title: "ログイン画面"
window:
  title: "ログイン"
  width: 400
  height: 300
  controls:
    - type: label
      text: "ユーザー名:"
      x: 30
      y: 30
    - type: textbox
      id: username
      x: 120
      y: 25
      width: 200
```
````

**変換後（Markdown）：**

```markdown
<!-- BEGIN:yaml-screen -->
<!-- yaml-screen
（元の YAML がコメントとして保持される）
yaml-screen -->

![ログイン画面](.screen-temp/example.svg)

<!-- END:yaml-screen -->
```

---

## restore

`transform` で変換済みのブロックを元の ` ```yaml-screen ` コードブロックに復元します。

```bash
screen-sketch restore <input-path>
```

| 引数           | 必須 | 説明                                |
| -------------- | ---- | ----------------------------------- |
| `<input-path>` | ○    | Markdown ファイルまたはディレクトリ |

---

## render

stdin から YAML を読み取り、stdout に SVG を出力します。VS Code 拡張や外部ツールからのパイプ連携に使用します。

```bash
screen-sketch render [--theme <name>] < input.yaml > output.svg
```

| 引数      | 必須 | 説明                                           |
| --------- | ---- | ---------------------------------------------- |
| `--theme` |      | カラーテーマ（`default`, `dark`, `blueprint`） |

YAML は標準入力から読み取り、SVG は標準出力に書き出します。
