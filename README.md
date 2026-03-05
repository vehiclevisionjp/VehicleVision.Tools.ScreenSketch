# VehicleVision.Tools.ScreenSketch

YAML で画面レイアウトを定義し、SVG 画像 + Markdown ドキュメントを自動生成するツールです。

## 機能

- **generate**: YAML ファイルから SVG 画面イメージ + Markdown ドキュメントを新規生成
- **transform**: Markdown 内の ` ```yaml-screen ` コードブロックを SVG + テーブルにインライン変換
- **restore**: 変換済みブロックを元の ` ```yaml-screen ` コードブロックに復元
- **render**: stdin から YAML を読み取り、stdout に SVG を出力（VS Code 拡張等のパイプ連携用）
- **VS Code 拡張**: Markdown プレビューで `yaml-screen` ブロックをリアルタイム SVG 表示

## インストール

### .NET グローバルツール（推奨）

```bash
dotnet tool install --global VehicleVision.Tools.ScreenSketch
```

### .NET ローカルツール

プロジェクト単位で管理する場合：

```bash
# ツールマニフェストを作成（初回のみ）
dotnet new tool-manifest

# ローカルツールとしてインストール
dotnet tool install VehicleVision.Tools.ScreenSketch
```

### ソースからビルド

```bash
git clone https://github.com/vehiclevisionjp/VehicleVision.Tools.ScreenSketch.git
cd VehicleVision.Tools.ScreenSketch
dotnet build
```

## 使い方

グローバルツールとしてインストールした場合：

```bash
# YAML から SVG + Markdown を生成
screen-sketch generate <input-path> [output-dir]

# Markdown 内の yaml-screen ブロックを変換
screen-sketch transform <input-path> [output-dir] [--inline]

# 変換済みブロックを復元
screen-sketch restore <input-path>

# stdin から YAML を読み取り、stdout に SVG を出力
echo '<yaml>' | screen-sketch render
```

ソースから実行する場合：

```bash
dotnet run --project VehicleVision.Tools.ScreenSketch -- generate <input-path> [output-dir]
```

## 画面イメージのサンプル

![コントロール一覧](images/all-controls.svg)

その他のサンプルは [Samples](VehicleVision.Tools.ScreenSketch/Samples/) ディレクトリを参照してください。

## ドキュメント

詳細なドキュメントは [Wiki](../../wiki) を参照してください。

- [コマンドリファレンス](../../wiki/Command-Reference) — 各コマンドの使い方と引数
- [YAML 定義リファレンス](../../wiki/YAML-Definition-Reference) — ドキュメント構造・screen・window・annotations
- [対応コントロール](../../wiki/Controls) — 全21コントロールのプロパティと画像
- [YAML 定義の例](../../wiki/Examples) — 最小構成・アノテーション付き・部分描画
- [VS Code 拡張](../../wiki/VS-Code-Extension) — Screen Sketch Preview の導入と設定

## プロジェクト構成

```text
VehicleVision.Tools.ScreenSketch/
├── .github/                    # GitHub設定（CI/CD、セキュリティポリシー等）
│   ├── copilot-instructions.md
│   ├── SECURITY.md
│   └── workflows/
│       ├── ci.yml
│       ├── release.yml
│       └── sync-wiki.yml
├── .vscode/                    # VS Code設定
│   ├── extensions.json
│   ├── settings.json
│   └── tasks.json
├── docs/                       # ドキュメント
│   ├── contributing/           # 開発者向けガイドライン
│   ├── script/                 # Wiki同期スクリプト
│   └── wiki/                   # Wikiソース（GitHub Wikiへ同期）
├── VehicleVision.Tools.ScreenSketch/
│   ├── Generation/             # Markdown生成ロジック
│   ├── Models/                 # YAML定義モデル
│   ├── Rendering/              # SVGレンダリング・テーマ
│   └── Samples/                # サンプルYAMLファイル
├── VehicleVision.Tools.ScreenSketch.VSCode/  # VS Code拡張（Markdownプレビュー連携）
│   ├── src/
│   │   └── extension.ts        # markdown-itプラグイン
│   ├── package.json
│   └── tsconfig.json
├── LICENSES/                   # サードパーティライセンス
├── .editorconfig
├── .gitignore
├── .markdownlint-cli2.jsonc
├── .prettierignore
├── .prettierrc
├── AUTHORS
├── CONTRIBUTING.md
├── LICENSE
├── README.md
└── package.json
```

## サードパーティライセンス

このプロジェクトは以下のサードパーティライブラリを使用しています：

| ライブラリ | ライセンス | 著作権                      |
| ---------- | ---------- | --------------------------- |
| YamlDotNet | MIT        | Copyright (c) Antoine Aubry |

ライセンスファイルの全文は [LICENSES](./LICENSES/) フォルダを参照してください。

## セキュリティ

セキュリティ上の脆弱性を発見された場合は、[セキュリティポリシー](.github/SECURITY.md)をご確認の上、ご報告ください。

## ライセンス

このプロジェクトは AGPL-3.0 ライセンスの下で公開されています。詳細は [LICENSE](LICENSE) を参照してください。

## 謝辞

セキュリティ脆弱性の報告やプロジェクトへの貢献をしてくださった方々に感謝いたします。

## 著作権

Copyright (c) 株式会社ピー・エム・シー ITソリューション事業部

<!-- 貢献者・報告者はこちらに追記 -->
