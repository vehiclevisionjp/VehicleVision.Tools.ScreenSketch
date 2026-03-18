# ドキュメントガイドライン

このドキュメントでは、VehicleVision.Tools.ScreenSketch プロジェクトのドキュメント作成規約について説明します。

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

---

## 基本原則

### 言語

- ドキュメントは**日本語**で記述する
- コード内のコメント（XMLドキュメントコメント含む）も日本語

### 対象読者とドキュメントの役割

各ドキュメントの対象読者と役割を明確に分離する。

| ドキュメント | 対象読者 | 役割 |
| ------------ | -------- | ---- |
| `README.md` | **利用者** | インストール方法、使い方、対応コントロール、ライセンス情報 |
| `CONTRIBUTING.md` | **開発者** | 開発環境構築、プロジェクト構成、ブランチ戦略、コーディング規約 |
| `docs/wiki/` | **利用者** | YAML定義リファレンス、コマンド詳細、コントロール仕様、サンプル |
| `docs/contributing/` | **開発者** | 各種ガイドライン（コーディング、テスト、ドキュメント、CI/CD） |

- `README.md` には開発者向け情報（プロジェクト構成、ソースからのビルド手順等）を記載しない
- `CONTRIBUTING.md` にプロジェクト構成やビルド手順を集約する
- `README.md` から Wiki および `CONTRIBUTING.md` へのリンクで誘導する

---

## ファイル構成

### ディレクトリ構造

```text
docs/
├── contributing/
│   ├── branch-strategy.md
│   ├── ci-workflow.md
│   ├── coding-guidelines.md
│   ├── development-environment.md
│   ├── documentation-guidelines.md
│   └── testing-guidelines.md
├── script/
│   ├── decode-toc.js
│   ├── generate-pdf.js
│   ├── github-markdown.css
│   ├── sync-docs-to-wiki.js
│   └── toc-single.js
└── wiki/
    ├── Home.md
    ├── Architecture.md
    ├── Command-Reference.md
    ├── Controls.md
    ├── Examples.md
    ├── VS-Code-Extension.md
    ├── YAML-Definition-Reference.md
    └── images/
```

### Wiki構成

`docs/wiki/` 配下は以下の体系で構成する。

| ページ | 内容 |
| ------ | ---- |
| `Home.md` | Wikiトップページ。全ページへのナビゲーション |
| `Command-Reference.md` | CLIコマンドの引数・オプション詳細 |
| `YAML-Definition-Reference.md` | YAMLスキーマの全プロパティ解説 |
| `Controls.md` | 対応コントロールの種類と設定 |
| `Examples.md` | 画面定義のサンプルと出力例 |
| `Architecture.md` | 内部構造とレンダリングパイプライン |
| `VS-Code-Extension.md` | VS Code拡張のインストールと使い方 |

### ファイル命名規則

#### `docs/wiki/` 配下

- ケバブケース（`example-document.md`）
- カテゴリごとにサブディレクトリで整理

#### `docs/contributing/` 配下

- ケバブケース（`coding-guidelines.md`）

---

## Markdownスタイル

### 基本ルール

| ルール         | 内容                                       |
| -------------- | ------------------------------------------ |
| 見出し         | ATX形式（`# H1`）を使用                   |
| 見出しレベル   | 1つずつ順番に（H1 → H2 → H3）            |
| リスト         | `-` を使用（`*` は不可）                  |
| コードブロック | バッククォート3つで囲む                   |
| 行の長さ       | 120文字以内（テーブル・コードブロック除外） |
| 絵文字         | 使用しない                                 |
| HTMLタグ       | 原則使用しない（`<br>` のみ許可）          |
| 型名           | バッククォートで囲む（例: \`string\`）     |

### フォーマッター（Prettier）

```bash
# フォーマット実行
npm run format

# フォーマットチェック（変更なし）
npm run format:check
```

### Linter（markdownlint）

```bash
# Lintチェック
npm run lint:md

# 自動修正
npm run lint:md:fix
```

---

## Wikiリンク規約

`docs/wiki/` 配下のファイルは CI（`sync-wiki.yml`）によって GitHub Wiki リポジトリへ自動同期される。
そのため、Wiki ページ間のリンクは以下のルールに従うこと。

### ページ間リンク

Wiki ページ間のリンクには **`.md` 拡張子を付けない**形式を使用する。

```markdown
<!-- ○ 正しい形式 -->
[コマンドリファレンス](Command-Reference)
[YAML定義リファレンス](YAML-Definition-Reference)
[Home](Home)

<!-- × 誤った形式 -->
[コマンドリファレンス](Command-Reference.md)
[コマンドリファレンス](docs/wiki/Command-Reference.md)
```

### 理由

- GitHub Wiki では `.md` 拡張子なしの相対リンクでページ間遷移が行われる
- リポジトリ内の `docs/wiki/` からの相対パスではなく、Wiki 上のページ名で解決される
- `.md` 拡張子を付けると、Wiki 上でリンクが正しく動作しない場合がある

### 画像リンク

Wiki 内の画像は `images/` サブディレクトリに配置し、相対パスで参照する。

```markdown
![説明](images/sample-screen.svg)
```

### 外部リンク

リポジトリ内の他のファイル（`README.md`、`CONTRIBUTING.md` 等）へのリンクは絶対URLを使用する。

```markdown
[README](https://github.com/vehiclevisionjp/VehicleVision.Tools.ScreenSketch/blob/main/README.md)
```

---

## npmスクリプト

| スクリプト     | 説明                             |
| -------------- | -------------------------------- |
| `lint:md`      | Markdownの構文チェック           |
| `lint:md:fix`  | Markdownのlintエラーを自動修正   |
| `format`       | Prettierでフォーマット           |
| `format:check` | フォーマットのチェック           |
| `toc`          | doctocでTOCを一括更新            |
| `toc:all`      | TOC更新 + フォーマットを一括実行 |
| `pdf`          | 全MarkdownをPDFに変換            |
| `pdf:wiki`     | WikiドキュメントのみPDF変換      |

---

## TOC（目次）自動生成

[doctoc](https://github.com/thlorenz/doctoc) を使用してTOCを自動生成する。

```bash
# 全ファイルのTOC更新
npm run toc

# TOC更新 + フォーマット
npm run toc:all
```

VS Codeでは **RunOnSave** 拡張機能により、`docs/` 配下のMarkdownファイル保存時にTOCが自動更新される。

---

## ドキュメント同期

### Wiki同期（CI）

`docs/wiki/` 配下のファイルは `sync-wiki.yml` ワークフローにより GitHub Wiki リポジトリへ自動同期される。
同期対象は `docs/wiki/` ディレクトリ内の全ファイル（画像含む）。

### 更新ルール

| 変更内容                       | 更新が必要なドキュメント                                   |
| ------------------------------ | ---------------------------------------------------------- |
| 公開APIの追加・変更            | `docs/wiki/` 配下の該当ドキュメント                        |
| コントロールの追加・変更       | `docs/wiki/Controls.md`、`docs/wiki/YAML-Definition-Reference.md` |
| CLIコマンドの追加・変更        | `docs/wiki/Command-Reference.md`                           |
| テーマ・レンダリングの変更     | `docs/wiki/` 配下の該当ドキュメント、サンプル画像の再生成  |
| VS Code拡張機能の変更          | `docs/wiki/VS-Code-Extension.md`                           |
| ガイドラインの追加             | `CONTRIBUTING.md` および `.github/copilot-instructions.md` |
| プロジェクト設定の変更         | `README.md` および `.github/copilot-instructions.md`       |
| セキュリティ脆弱性の報告対応   | `README.md` の謝辞セクション（報告者名を追記）             |
