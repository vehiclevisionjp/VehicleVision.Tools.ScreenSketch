# VehicleVision.Tools.ScreenSketch

YAML で画面レイアウトを定義し、SVG 画像 + Markdown ドキュメントを自動生成するツールです。

## 機能

- **generate**: YAML ファイルから SVG 画面イメージ + Markdown ドキュメントを新規生成
- **transform**: Markdown 内の ` ```yaml-screen ` コードブロックを SVG + テーブルにインライン変換
- **restore**: 変換済みブロックを元の ` ```yaml-screen ` コードブロックに復元

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
```

ソースから実行する場合：

```bash
dotnet run --project VehicleVision.Tools.ScreenSketch -- generate <input-path> [output-dir]
```

## サンプル

[Samples/sample-screen.yaml](VehicleVision.Tools.ScreenSketch/Samples/sample-screen.yaml) に車両検索画面のサンプル定義があります。

## 対応コントロール

button, textbox, label, combobox, checkbox, radiobutton, group, datagrid, menubar, statusbar, tabcontrol, listbox, panel, image, progressbar, numericupdown, datetimepicker, treeview, toolbar, linklabel, textarea

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
│   ├── script/                 # ドキュメント用スクリプト
│   └── wiki/                   # Wikiドキュメント
├── VehicleVision.Tools.ScreenSketch/
│   ├── Generation/             # Markdown生成ロジック
│   ├── Models/                 # YAML定義モデル
│   ├── Rendering/              # SVGレンダリング・テーマ
│   └── Samples/                # サンプルYAMLファイル
├── LICENSES/                   # サードパーティライセンス
├── .editorconfig
├── .gitignore
├── .markdownlint-cli2.jsonc
├── .prettierignore
├── .prettierrc
├── AUTHORS
├── CONTRIBUTING.md
├── Directory.Build.props
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

<!-- 貢献者・報告者はこちらに追記 -->
