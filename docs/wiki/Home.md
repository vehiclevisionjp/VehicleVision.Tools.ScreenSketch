# VehicleVision.Tools.ScreenSketch Wiki

YAML で画面レイアウトを定義し、SVG 画像 + Markdown ドキュメントを自動生成するツールのドキュメントです。

## ページ一覧

| ページ | 内容 |
| ------ | ---- |
| [コマンドリファレンス](Command-Reference) | CLI コマンドの詳細な使い方 |
| [YAML定義リファレンス](YAML-Definition-Reference) | YAML スキーマの全プロパティ解説 |
| [コントロール一覧](Controls) | 対応コントロールの種類と設定 |
| [サンプル集](Examples) | 画面定義のサンプルと出力例 |
| [アーキテクチャ](Architecture) | 内部構造とレンダリングパイプライン |
| [VS Code 拡張機能](VS-Code-Extension) | VS Code 拡張のインストールと使い方 |

## クイックスタート

### インストール

```bash
dotnet tool install --global VehicleVision.Tools.ScreenSketch
```

### 基本的な使い方

```bash
# YAML から SVG + Markdown を生成
screen-sketch generate input.yaml output/

# Markdown 内の yaml-screen ブロックを変換
screen-sketch transform input.md output/ [--inline]

# 変換済みブロックを復元
screen-sketch restore input.md
```

詳細は[コマンドリファレンス](Command-Reference)を参照してください。
