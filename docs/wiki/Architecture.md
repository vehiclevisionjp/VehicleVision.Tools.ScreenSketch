# アーキテクチャ

## プロジェクト全体像

本プロジェクトは、YAML で画面レイアウトを定義し SVG + Markdown を生成するツールです。
同じ「YAML → SVG 変換ロジック」を **2 つの言語で独立して実装** しています。

```
VehicleVision.Tools.ScreenSketch/          ← C# 実装（.NET CLI ツール）
VehicleVision.Tools.ScreenSketch.VSCode/   ← TypeScript 実装（VS Code 拡張）
```

---

## なぜ 2 つの実装があるのか

### 変更前の構成（外部プロセス呼び出し）

もともと VS Code 拡張は、レンダリングロジックを自分では持たず、
.NET CLI ツール `screen-sketch` を **外部プロセスとして呼び出す** 構成でした。

```
┌──────────────────────────┐
│   VS Code 拡張           │
│   (TypeScript)           │
│                          │
│   yaml-screen ブロック    │
│         │                │
│         ▼                │
│   execFileSync(          │
│     "screen-sketch",     │  ← 外部プロセス呼び出し
│     ["render"],          │
│     { input: yaml }      │
│   )                      │
│         │                │
│         ▼                │
│   SVG 文字列を受け取る    │
└──────────┬───────────────┘
           │
           ▼
┌──────────────────────────┐
│   .NET CLI ツール         │
│   (C# / .NET 10)         │
│                          │
│   SvgRenderer.cs         │  ← 実際のレンダリング処理
│   ThemeColors.cs         │
│   ScreenDefinition.cs    │
└──────────────────────────┘
```

この構成には以下の問題がありました：

- ユーザーが事前に `dotnet tool install -g VehicleVision.Tools.ScreenSketch` を実行する必要がある
- .NET ランタイムのインストールも必要
- プロセス起動のオーバーヘッドがプレビュー表示のたびに発生する

### 変更後の構成（レンダリング一体化）

C# の SVG レンダリングロジックを **TypeScript に移植（書き直し）** し、
VS Code 拡張内に組み込みました。

```
┌──────────────────────────┐
│   VS Code 拡張           │
│   (TypeScript)           │
│                          │
│   yaml-screen ブロック    │
│         │                │
│         ▼                │
│   js-yaml で YAML 解析   │  ← 拡張内で完結
│         │                │
│         ▼                │
│   SvgRenderer (TS)       │  ← TypeScript に移植した
│   ThemeColors (TS)       │    レンダリングエンジン
│         │                │
│         ▼                │
│   SVG 文字列             │
└──────────────────────────┘

┌──────────────────────────┐
│   .NET CLI ツール         │  ← 引き続き独立して利用可能
│   (C# / .NET 10)         │    （generate / transform /
│                          │     restore コマンド用）
│   SvgRenderer.cs         │
│   ThemeColors.cs         │
│   ScreenDefinition.cs    │
└──────────────────────────┘
```

---

## よくある疑問

### Q. TypeScript で C# の DLL を読み込んでいるの？

**いいえ。** TypeScript から C# の DLL やアセンブリを直接読み込むことはしていません。

やったことは、C# で書かれたレンダリングロジックを **TypeScript で書き直した**（移植した）ということです。
具体的には、以下の C# ファイルを参考に、同等のロジックを TypeScript で再実装しています：

| C# ファイル（.NET CLI ツール） | TypeScript ファイル（VS Code 拡張） | 内容 |
| --- | --- | --- |
| `Rendering/SvgRenderer.cs`（876 行） | `src/svgRenderer.ts`（1006 行） | SVG レンダリングエンジン |
| `Rendering/ThemeColors.cs`（206 行） | `src/themeColors.ts`（272 行） | テーマカラー定義 |
| `Models/ScreenDefinition.cs`（157 行） | `src/models.ts`（118 行） | YAML データモデル |

C# と TypeScript の各メソッドは 1:1 で対応しています。
たとえば `RenderButton`（C#）→ `renderButton`（TypeScript）のように、
関数名・引数・処理内容がほぼ同じになっています。

### Q. なぜ DLL 読み込みではなく書き直しなのか？

技術的に TypeScript（Node.js）から .NET の DLL を呼び出す方法は存在しますが
（例: Edge.js、WebAssembly 経由など）、以下の理由から「書き直し」を選択しています：

1. **依存関係の最小化** — .NET ランタイム不要、VS Code だけで動作する
2. **パフォーマンス** — プロセス間通信のオーバーヘッドがない
3. **導入の容易さ** — VS Code 拡張をインストールするだけで使える
4. **保守性** — ネイティブ連携のブリッジコードが不要

### Q. 2 つの実装を同期させるのが大変では？

はい、その通りです。これはトレードオフです。

C# 側にコントロールの追加やレンダリングの変更を加えた場合、
TypeScript 側にも同じ変更を反映する必要があります。

ただし、レンダリングロジック自体は比較的安定しており、頻繁に変更されるものではないため、
現時点では許容範囲と判断しています。

---

## .NET CLI ツールの役割

VS Code 拡張にレンダリングエンジンが一体化された後も、
.NET CLI ツール（`screen-sketch` コマンド）は以下の用途で引き続き使用されます：

| コマンド | 用途 |
| --- | --- |
| `generate` | YAML → SVG ファイル + Markdown ファイルの一括生成 |
| `transform` | Markdown 内の `yaml-screen` ブロックを SVG + テーブルに変換 |
| `restore` | 変換済みブロックをコードブロックに復元 |
| `render` | stdin YAML → stdout SVG（パイプライン向け） |

VS Code 拡張はあくまで **Markdown プレビュー** での表示に特化しており、
ファイル生成やバッチ処理には .NET CLI ツールを使用します。
