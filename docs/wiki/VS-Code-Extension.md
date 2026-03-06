# VS Code 拡張（Screen Sketch Preview）

Markdown プレビューで ` ```yaml-screen ` コードブロックを SVG 画面イメージとしてリアルタイム表示する VS Code 拡張です。

---

## インストール

### マーケットプレイスから（推奨）

VS Code マーケットプレイスからインストールすると、レンダリングエンジンがバンドルされているため追加の設定は不要です。

### 開発ビルドの場合

バンドル済みバイナリがない場合、.NET CLI ツールが必要です：

```bash
dotnet tool install -g VehicleVision.Tools.ScreenSketch
```

---

## 使い方

Markdown ファイルに `yaml-screen` コードブロックを記述すると、Markdown プレビューに SVG 画面イメージが表示されます。

````markdown
```yaml-screen
screen:
  title: サンプル画面
window:
  title: ログイン
  width: 400
  height: 200
  controls:
    - type: label
      text: "ユーザー名:"
      x: 20
      y: 20
    - type: textbox
      x: 120
      y: 15
      width: 200
```
````

---

## 設定

| 設定                    | デフォルト      | 説明                         |
| ----------------------- | --------------- | ---------------------------- |
| `screenSketch.toolPath` | `screen-sketch` | screen-sketch コマンドのパス |
| `screenSketch.theme`    | （空）          | SVG レンダリングに使用するカラーテーマ (`default`, `dark`, `blueprint`)。空の場合は YAML 内の設定が使われます |

---

## 開発

```bash
cd VehicleVision.Tools.ScreenSketch.VSCode
npm install
npm run compile
```

F5 で Extension Development Host を起動してテストできます。
開発時はグローバルインストールされた `screen-sketch` コマンドが使用されます。

---

## マーケットプレイス向けパッケージング

プラットフォーム別にバイナリをバンドルした VSIX を生成します：

```bash
npm run package:win32-x64      # Windows x64
npm run package:linux-x64      # Linux x64
npm run package:darwin-x64     # macOS x64
npm run package:darwin-arm64   # macOS ARM64 (Apple Silicon)
```

詳しくは [アーキテクチャ](Architecture.md) を参照してください。
