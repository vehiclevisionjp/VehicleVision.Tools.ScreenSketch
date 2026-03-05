# VS Code 拡張（Screen Sketch Preview）

Markdown プレビューで ` ```yaml-screen ` コードブロックを SVG 画面イメージとしてリアルタイム表示する VS Code 拡張です。

---

## 前提条件

`screen-sketch` ツールがインストール済みである必要があります。拡張は内部で `screen-sketch render` コマンドを呼び出して SVG を生成します。

---

## インストール

```bash
cd vscode-screen-sketch
npm install
npm run compile
```

VS Code で `vscode-screen-sketch` フォルダを開き、F5 で Extension Development Host を起動してテストできます。

---

## 設定

| 設定                    | デフォルト      | 説明                         |
| ----------------------- | --------------- | ---------------------------- |
| `screenSketch.toolPath` | `screen-sketch` | screen-sketch コマンドのパス |

詳細は [vscode-screen-sketch/README.md](https://github.com/vehiclevisionjp/VehicleVision.Tools.ScreenSketch/blob/main/vscode-screen-sketch/README.md) を参照してください。
