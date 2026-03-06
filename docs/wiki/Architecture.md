# アーキテクチャ

## プロジェクト全体像

本プロジェクトは、YAML で画面レイアウトを定義し SVG + Markdown を生成するツールです。

```
VehicleVision.Tools.ScreenSketch/          ← C# 実装（.NET CLI ツール / レンダリングエンジン）
VehicleVision.Tools.ScreenSketch.VSCode/   ← VS Code 拡張（TypeScript / プロセス呼び出しのみ）
```

**レンダリングロジックは C# の 1 箇所のみで管理** されています。
VS Code 拡張はレンダリングロジックを持たず、C# ビルド済みバイナリを呼び出します。

---

## レンダリングの仕組み

VS Code 拡張は `screen-sketch render` コマンドを **プロセス呼び出し** で実行し、
stdin に YAML を渡して stdout から SVG を受け取ります。

```
┌──────────────────────────────────────────┐
│   VS Code 拡張 (TypeScript)              │
│                                          │
│   yaml-screen ブロック                    │
│         │                                │
│         ▼                                │
│   resolveToolPath()                      │
│     1. bin/ にバイナリが存在？ → 使用     │  ← マーケット公開版
│     2. なければ設定 or グローバルツール     │  ← 開発時
│         │                                │
│         ▼                                │
│   execFileSync(toolPath, ["render"])      │
│         │                                │
│         ▼                                │
│   SVG 文字列を受け取り表示                │
└──────────┬───────────────────────────────┘
           │
           ▼
┌──────────────────────────────────────────┐
│   screen-sketch バイナリ (C# / .NET)     │
│                                          │
│   SvgRenderer.cs    ← レンダリングの実体  │
│   ThemeColors.cs                         │
│   ScreenDefinition.cs                    │
└──────────────────────────────────────────┘
```

---

## マーケットプレイス公開方式

VS Code マーケットプレイスは **プラットフォーム別 VSIX パッケージ** に対応しています。
各プラットフォーム用に `dotnet publish --self-contained` で生成した
ネイティブバイナリを拡張にバンドルして公開します。

### VSIX パッケージの中身

```
拡張パッケージ（例: win32-x64 用 VSIX）
├── out/extension.js                ← TypeScript（呼び出し側のみ）
├── bin/VehicleVision.Tools.ScreenSketch    ← C# 自己完結型バイナリ
├── package.json
└── ...
```

### パッケージング手順

```bash
cd VehicleVision.Tools.ScreenSketch.VSCode

# Windows x64 向け
npm run package:win32-x64

# Linux x64 向け
npm run package:linux-x64

# macOS x64 向け
npm run package:darwin-x64

# macOS ARM64 向け（Apple Silicon）
npm run package:darwin-arm64
```

各コマンドは内部で以下を実行します：

1. `dotnet publish` — C# プロジェクトを対象プラットフォーム向けに自己完結型バイナリとしてビルド
2. `vsce package --target <platform>` — プラットフォーム別 VSIX を生成

### 開発時のフォールバック

バンドル済みバイナリが存在しない場合（開発環境）、拡張は
`screenSketch.toolPath` の設定値またはグローバルインストールされた
`screen-sketch` コマンドにフォールバックします。

---

## なぜこの方式か — 検討した代替案

| 方式 | ソース管理 | .NET 必要？ | 採否 | 理由 |
| --- | --- | --- | --- | --- |
| **バイナリバンドル** | C# のみ ✅ | 不要 ✅ | ✅ **採用** | ソース一元管理 + ユーザー体験◎ |
| C# → TypeScript に書き直し | 2箇所 ❌ | 不要 | ❌ | 二重管理が発生する |
| C# → WebAssembly | C# のみ ✅ | 不要 | ❌ | .NET WASM 対応が実験的 |
| 外部ツール手動インストール | C# のみ ✅ | 必要 ❌ | ❌ | マーケット公開に不適 |

---

## .NET CLI ツールのコマンド体系

| コマンド | 用途 |
| --- | --- |
| `generate` | YAML → SVG ファイル + Markdown ファイルの一括生成 |
| `transform` | Markdown 内の `yaml-screen` ブロックを SVG + テーブルに変換 |
| `restore` | 変換済みブロックをコードブロックに復元 |
| `render` | stdin YAML → stdout SVG（VS Code 拡張から呼び出される） |
