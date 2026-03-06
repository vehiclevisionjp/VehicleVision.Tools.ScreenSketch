# 開発環境構築ガイド

このドキュメントでは、VehicleVision.Tools.ScreenSketch プロジェクトの開発環境セットアップについて説明します。

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

---

## 必要なツール

| ツール   | 用途                             | 必須 |
| -------- | -------------------------------- | ---- |
| .NET SDK | ツールのビルド・テスト           | 必須 |
| Node.js  | ドキュメントのlint・フォーマット | 推奨 |
| VS Code  | 推奨エディタ                     | 推奨 |
| Git      | バージョン管理                   | 必須 |

---

## .NET SDK

### インストール

[.NET SDK 10.0](https://dotnet.microsoft.com/download) 以上をインストールしてください。

### バージョン確認

```bash
dotnet --version
```

---

## Node.js（推奨）

ドキュメントの lint・フォーマットに使用します。必須ではありませんが推奨です。

### インストール

[Node.js](https://nodejs.org/) の最新LTS版をインストールしてください。

### パッケージのインストール

```bash
npm install
```

---

## IDE（Visual Studio Code）

### 推奨拡張機能

本プロジェクトでは以下の拡張機能を推奨している（`.vscode/extensions.json` に定義済み）。

| 拡張機能ID                       | 名称         | 用途                 |
| -------------------------------- | ------------ | -------------------- |
| `ms-dotnettools.csharp`          | C#           | C#言語サポート       |
| `ms-dotnettools.csdevkit`        | C# Dev Kit   | .NET開発の統合支援   |
| `esbenp.prettier-vscode`         | Prettier     | コードフォーマッター |
| `editorconfig.editorconfig`      | EditorConfig | エディタ設定の統一   |
| `davidanson.vscode-markdownlint` | markdownlint | Markdown lint        |

### ビルドタスク

プロジェクトには `.vscode/tasks.json` が含まれており、VS Code のタスク機能でビルドやドキュメント操作を実行できる。

#### ビルドタスク

| タスク            | 説明                         |
| ----------------- | ---------------------------- |
| `restore`         | NuGetパッケージの復元        |
| `build`           | プロジェクトのビルド（既定） |
| `build (Release)` | リリースビルド               |
| `clean`           | ビルド成果物のクリーン       |
| `rebuild`         | クリーン → 復元 → ビルド     |
| `format`          | C#コードのフォーマット       |

#### ドキュメントタスク（npm）

| タスク              | 説明                               |
| ------------------- | ---------------------------------- |
| `npm: lint:md`      | Markdownの構文チェック             |
| `npm: lint:md:fix`  | Markdownのlintエラーを自動修正     |
| `npm: format`       | Prettierでフォーマット             |
| `npm: format:check` | フォーマットのチェック（変更なし） |
| `npm: toc`          | doctocでTOCを一括更新              |

#### タスクの実行方法

`Ctrl+Shift+P` → `Tasks: Run Task` から選択。

---

## セットアップ確認

以下のコマンドですべてが正しくセットアップされていることを確認する。

```bash
# .NET SDK
dotnet --version

# Node.js（推奨）
node --version
npm --version

# ビルド確認
dotnet restore
dotnet build
```

---

## 参考リンク

- [.NET ドキュメント](https://learn.microsoft.com/ja-jp/dotnet/)
- [Node.js 公式サイト](https://nodejs.org/ja/)
- [Visual Studio Code ドキュメント](https://code.visualstudio.com/docs)
