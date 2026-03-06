# VS Code 拡張（Screen Sketch Preview）

Markdown プレビューで ` ```yaml-screen ` コードブロックを SVG 画面イメージとしてリアルタイム表示する VS Code 拡張です。
レンダリングエンジンを拡張内に組み込んでいるため、外部ツールのインストールは不要です。

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

| 設定                 | デフォルト | 説明                                                                                    |
| -------------------- | ---------- | --------------------------------------------------------------------------------------- |
| `screenSketch.theme` | （空）     | SVG レンダリングに使用するカラーテーマ (`default`, `dark`, `blueprint`)。空の場合は YAML 内の設定が使われます |

---

## 技術的な仕組み

この拡張は、.NET CLI ツール (`screen-sketch`) の C# レンダリングロジックを **TypeScript に移植** して内蔵しています。
外部プロセスの呼び出しや .NET ランタイムは不要です。

| C# 実装（.NET CLI ツール） | TypeScript 実装（本拡張） | 内容 |
| --- | --- | --- |
| `Rendering/SvgRenderer.cs` | `src/svgRenderer.ts` | SVG レンダリングエンジン |
| `Rendering/ThemeColors.cs` | `src/themeColors.ts` | テーマカラー定義 |
| `Models/ScreenDefinition.cs` | `src/models.ts` | YAML データモデル |

YAML の解析には [js-yaml](https://www.npmjs.com/package/js-yaml) を使用しています。

> **注:** C# の DLL を直接読み込んでいるわけではありません。
> C# のロジックを参考に、同等の処理を TypeScript で再実装しています。
> 詳しくは [アーキテクチャ](Architecture.md) を参照してください。

---

## 開発

```bash
cd VehicleVision.Tools.ScreenSketch.VSCode
npm install
npm run compile
```

F5 で Extension Development Host を起動してテストできます。
