# コマンドリファレンス

VehicleVision.Tools.ScreenSketch の CLI コマンド一覧です。

## generate

YAML ファイルから SVG 画面イメージと Markdown ドキュメントを生成します。

```bash
screen-sketch generate <input-path> [output-dir]
```

| 引数 | 必須 | 説明 |
| ---- | ---- | ---- |
| `input-path` | ○ | 入力 YAML ファイルまたはディレクトリのパス |
| `output-dir` | - | 出力先ディレクトリ（省略時は入力と同じディレクトリ） |

## transform

Markdown 内の ` ```yaml-screen ` コードブロックを SVG + テーブルにインライン変換します。

```bash
screen-sketch transform <input-path> [output-dir] [--inline]
```

| 引数・オプション | 必須 | 説明 |
| ---------------- | ---- | ---- |
| `input-path` | ○ | 入力 Markdown ファイルのパス |
| `output-dir` | - | 出力先ディレクトリ（省略時は入力と同じディレクトリ） |
| `--inline` | - | 元ファイルを直接書き換える |

## restore

変換済みブロックを元の ` ```yaml-screen ` コードブロックに復元します。

```bash
screen-sketch restore <input-path>
```

| 引数 | 必須 | 説明 |
| ---- | ---- | ---- |
| `input-path` | ○ | 復元対象の Markdown ファイルのパス |

## 関連ページ

- [YAML定義リファレンス](YAML-Definition-Reference)
- [サンプル集](Examples)
- [Home](Home)
