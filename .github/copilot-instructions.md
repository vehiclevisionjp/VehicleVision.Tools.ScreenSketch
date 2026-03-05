# Copilot Instructions

このリポジトリは **VehicleVision.Tools.ScreenSketch** - YAML で画面レイアウトを定義し、SVG 画像 + Markdown ドキュメントを自動生成するツールです。

## プロジェクト情報

- **ターゲット**: .NET 10
- **言語バージョン**: C# latest
- **Nullable**: 有効
- **JSONライブラリ**: System.Text.Json
- **YAMLライブラリ**: YamlDotNet

## プロジェクト構成

| ディレクトリ   | 内容                                   |
| -------------- | -------------------------------------- |
| `Generation/`  | Markdown 生成ロジック                  |
| `Models/`      | YAML 定義モデル（ScreenDefinition 等） |
| `Rendering/`   | SVG レンダリング・テーマ               |
| `Samples/`     | サンプル YAML ファイル                 |
| `docs/wiki/`   | Wiki 同期対象のドキュメント            |
| `docs/script/` | ドキュメント同期スクリプト             |

## 変更時のルール

- コードを変更する際には、`docs/wiki/` 配下の関連ドキュメントも併せて変更すること
- CI/CD ワークフローの変更時は、関連ドキュメントも更新すること

## 出力ルール

- 優先順位や処理の都合上、指示されたタスクの一部を実行しない・できない場合は、その旨を明示的に Prompt で出力すること
- 省略した内容と理由を簡潔に説明し、必要に応じて後続の対応を提案すること
