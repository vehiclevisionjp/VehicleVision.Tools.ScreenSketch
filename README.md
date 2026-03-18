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

## 使い方

```bash
# YAML から SVG + Markdown を生成
screen-sketch generate <input-path> [output-dir]

# Markdown 内の yaml-screen ブロックを変換
screen-sketch transform <input-path> [output-dir] [--inline]

# 変換済みブロックを復元
screen-sketch restore <input-path>
```

詳細なコマンドリファレンスは [Wiki](https://github.com/vehiclevisionjp/VehicleVision.Tools.ScreenSketch/wiki) を参照してください。

## 対応コントロール

button, textbox, label, combobox, checkbox, radiobutton, group, datagrid, menubar, statusbar, tabcontrol, listbox, panel, image, progressbar, numericupdown, datetimepicker, treeview, toolbar, linklabel, textarea

## ドキュメント

| ドキュメント | 内容 |
| ------------ | ---- |
| [Wiki](https://github.com/vehiclevisionjp/VehicleVision.Tools.ScreenSketch/wiki) | YAML定義リファレンス、コマンド詳細、サンプル等 |
| [CONTRIBUTING](CONTRIBUTING.md) | 開発者向けガイド（環境構築、コーディング規約、ブランチ戦略等） |

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
