# アーキテクチャ

## プロジェクト全体像

本プロジェクトは、YAML で画面レイアウトを定義し SVG + Markdown を生成するツールです。

```
VehicleVision.Tools.ScreenSketch/          ← C# 実装（.NET CLI ツール）
VehicleVision.Tools.ScreenSketch.VSCode/   ← VS Code 拡張（TypeScript）
```

---

## レンダリングの仕組み

VS Code 拡張は、レンダリングロジック自体を持たず、
.NET CLI ツール `screen-sketch render` を **外部プロセスとして呼び出す** 構成です。

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
│   SvgRenderer.cs         │  ← レンダリングの実体
│   ThemeColors.cs         │
│   ScreenDefinition.cs    │
└──────────────────────────┘
```

この構成により **ソースコードは C# の1箇所のみで管理** されます。

---

## なぜこの方式か — 検討した代替案

「外部プロセス呼び出しではなく、VS Code 拡張にレンダリングを一体化できないか？」
という検討を行い、以下の方式を比較しました。

| 方式 | ソース管理 | .NET 必要？ | 複雑度 | 採否 |
| --- | --- | --- | --- | --- |
| **外部プロセス呼び出し** | C# のみ ✅ | 必要 | 低 | ✅ **採用** |
| C# → TypeScript に書き直し | 2箇所 ❌ | 不要 | 低 | ❌ 二重管理 |
| C# → WebAssembly にコンパイル | C# のみ ✅ | 不要 | 高（実験的） | ❌ 時期尚早 |
| 自己完結バイナリをバンドル | C# のみ ✅ | 不要 | 中 | ❌ サイズ大 |

### 採用理由

- **ソースの一元管理**: レンダリングロジックは C# にのみ存在し、変更時の修正箇所が1箇所で済む
- **シンプルさ**: VS Code 拡張側はプロセス呼び出しのみで、拡張コードが小さく保てる
- **利用者の前提**: 本ツールの利用者は .NET 開発者であり、.NET ランタイムは導入済み

### 不採用とした方式の詳細

#### TypeScript への書き直し

C# のレンダリングロジックを TypeScript で再実装する方式。
外部ツール不要になるが、C# と TypeScript の両方にレンダリングコードを持つことになり、
変更時に2箇所の同期が必要になる。**二重管理は長期的にバグの温床になるため不採用。**

#### WebAssembly（WASM）

C# を WebAssembly にコンパイルし、VS Code 拡張から WASM として実行する方式。
ソースは C# のみで済むが、.NET の WASM ビルドはまだ実験的な段階であり、
ビルドパイプラインが複雑になる。.NET の WASM 対応が安定した段階で再検討する価値がある。

#### 自己完結型バイナリのバンドル

`dotnet publish --self-contained` で生成したネイティブバイナリを
VS Code 拡張パッケージに同梱する方式。
プラットフォーム別（Windows / macOS / Linux）のバイナリが必要で、
拡張パッケージのサイズが大幅に増加するため不採用。

---

## .NET CLI ツールのコマンド体系

| コマンド | 用途 |
| --- | --- |
| `generate` | YAML → SVG ファイル + Markdown ファイルの一括生成 |
| `transform` | Markdown 内の `yaml-screen` ブロックを SVG + テーブルに変換 |
| `restore` | 変換済みブロックをコードブロックに復元 |
| `render` | stdin YAML → stdout SVG（VS Code 拡張から呼び出される） |
