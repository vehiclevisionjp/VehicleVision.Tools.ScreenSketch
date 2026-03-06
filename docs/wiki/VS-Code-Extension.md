# VS Code 拡張（Screen Sketch Preview）

Markdown プレビューで ` ```yaml-screen ` コードブロックを SVG 画面イメージとしてリアルタイム表示する VS Code 拡張です。

---

## 前提条件

[VehicleVision.Tools.ScreenSketch](https://www.nuget.org/packages/VehicleVision.Tools.ScreenSketch) がインストールされている必要があります。
拡張は内部で `screen-sketch render` コマンドを呼び出して SVG を生成します。

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
