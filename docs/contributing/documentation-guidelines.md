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

### 対象読者

- プロジェクトの開発者・コントリビューター

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
```

### ファイル命名規則

#### `docs/contributing/` 配下

- ケバブケース（`coding-guidelines.md`）

---

## Markdownスタイル

### 基本ルール

| ルール         | 内容                                        |
| -------------- | ------------------------------------------- |
| 見出し         | ATX形式（`# H1`）を使用                     |
| 見出しレベル   | 1つずつ順番に（H1 → H2 → H3）               |
| リスト         | `-` を使用（`*` は不可）                    |
| コードブロック | バッククォート3つで囲む                     |
| 行の長さ       | 120文字以内（テーブル・コードブロック除外） |
| 絵文字         | 使用しない                                  |
| HTMLタグ       | 原則使用しない（`<br>` のみ許可）           |
| 型名           | バッククォートで囲む（例: `string`）        |

### フォーマッター（Prettier）

Prettierでフォーマットを統一する。設定は `.prettierrc` に定義済み。

```bash
npm run format
```

### Linter（markdownlint）

markdownlint-cli2でlintチェックを行う。設定は `.markdownlint-cli2.jsonc` に定義済み。

```bash
npm run lint:md
```

---

## npmスクリプト

| スクリプト     | 説明                               |
| -------------- | ---------------------------------- |
| `lint:md`      | Markdownの構文チェック             |
| `lint:md:fix`  | Markdownのlintエラーを自動修正     |
| `format`       | Prettierでフォーマット             |
| `format:check` | フォーマットのチェック（変更なし） |
| `toc`          | doctocでTOCを一括更新              |

---

## TOC（目次）自動生成

[doctoc](https://github.com/thlorenz/doctoc) を使用してTOCを自動生成する。

```bash
# 全ファイルのTOC更新
npm run toc
```

---

## ドキュメント同期

### 更新ルール

| 変更内容                     | 更新が必要なドキュメント                                   |
| ---------------------------- | ---------------------------------------------------------- |
| ガイドラインの追加           | `CONTRIBUTING.md` および `.github/copilot-instructions.md` |
| プロジェクト設定の変更       | `README.md` および `.github/copilot-instructions.md`       |
| セキュリティ脆弱性の報告対応 | `README.md` の謝辞セクション（報告者名を追記）             |
