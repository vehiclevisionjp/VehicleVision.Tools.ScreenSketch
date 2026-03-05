# VehicleVision.Tools.ScreenSketch

YAML で画面レイアウトを定義し、SVG 画像 + Markdown ドキュメントを自動生成するツールです。

## 機能

| コマンド    | 説明                                                                   |
| ----------- | ---------------------------------------------------------------------- |
| `generate`  | YAML ファイルから SVG 画面イメージ + Markdown ドキュメントを新規生成   |
| `transform` | Markdown 内の ` ```yaml-screen ` コードブロックを SVG + テーブルに変換 |
| `restore`   | 変換済みブロックを元の ` ```yaml-screen ` コードブロックに復元         |

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

## 対応コントロール

| コントロール     | 説明                     |
| ---------------- | ------------------------ |
| `button`         | ボタン                   |
| `textbox`        | テキストボックス         |
| `textarea`       | テキストエリア（複数行） |
| `label`          | ラベル                   |
| `combobox`       | コンボボックス           |
| `checkbox`       | チェックボックス         |
| `radiobutton`    | ラジオボタン             |
| `group`          | グループボックス         |
| `datagrid`       | データグリッド           |
| `menubar`        | メニューバー             |
| `statusbar`      | ステータスバー           |
| `tabcontrol`     | タブコントロール         |
| `listbox`        | リストボックス           |
| `panel`          | パネル                   |
| `image`          | 画像                     |
| `progressbar`    | プログレスバー           |
| `numericupdown`  | 数値アップダウン         |
| `datetimepicker` | 日付ピッカー             |
| `treeview`       | ツリービュー             |
| `toolbar`        | ツールバー               |
| `linklabel`      | リンクラベル             |

## プロジェクト構成

| ディレクトリ   | 内容                                   |
| -------------- | -------------------------------------- |
| `Generation/`  | Markdown 生成ロジック                  |
| `Models/`      | YAML 定義モデル（ScreenDefinition 等） |
| `Rendering/`   | SVG レンダリング・テーマ               |
| `Samples/`     | サンプル YAML ファイル                 |
| `docs/wiki/`   | Wiki 同期対象のドキュメント            |
| `docs/script/` | ドキュメント同期スクリプト             |

## ライセンス

このプロジェクトは GPL-3.0 ライセンスの下で公開されています。詳細はリポジトリの `LICENSE` を参照してください。
