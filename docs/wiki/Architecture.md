# アーキテクチャ

VehicleVision.Tools.ScreenSketch の内部構造とレンダリングパイプラインについて説明します。

## 全体構成

```text
YAML入力 → YAMLパーサー → ScreenDefinition → SvgRenderer → SVG出力
                                            → MarkdownGenerator → Markdown出力
```

## モジュール構成

| モジュール | ディレクトリ | 役割 |
| ---------- | ------------ | ---- |
| Models | `Models/` | YAML定義のデシリアライズモデル |
| Rendering | `Rendering/` | SVGレンダリング、テーマ、色解決 |
| Generation | `Generation/` | Markdownドキュメント生成 |

## レンダリングパイプライン

### 1. YAML解析

YamlDotNet を使用して YAML ファイルを `ScreenDefinition` モデルにデシリアライズします。

### 2. テーマ解決

`ThemeColors.FromName()` でテーマ名からカラーセットを解決します。
対応テーマ: default, dark, blueprint, custom

### 3. SVG生成

`SvgRenderer` が `ScreenDefinition` を受け取り、各コントロールをSVG要素に変換します。

### 4. Markdownドキュメント生成

`MarkdownGenerator` がコントロール定義からMarkdownテーブルを生成します。

## カラーシステム

- `#RRGGBB` 形式の16進数カラーコード
- `System.Drawing.KnownColor` の色名（例: `Control`, `ActiveBorder`, `Window`）

`ColorResolver` が色名を16進数コードに変換します。

## 関連ページ

- [YAML定義リファレンス](YAML-Definition-Reference)
- [コントロール一覧](Controls)
- [Home](Home)
