# コーディングガイドライン

このドキュメントでは、VehicleVision.Tools.ScreenSketch プロジェクトのコーディング規約について説明します。

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

---

## 基本原則

### プロジェクト設定

| 項目           | 設定             |
| -------------- | ---------------- |
| ターゲット     | .NET 10          |
| 言語           | C# latest        |
| Nullable       | enable           |
| JSONライブラリ | System.Text.Json |
| YAMLライブラリ | YamlDotNet       |
| 暗黙的using    | enable           |

### 重要な方針

- **Newtonsoft.Json は使用しない**（System.Text.Json を使用）
- **Nullable 参照型**を有効にし、null安全なコードを書く
- **file-scoped namespace** を使用する
- すべての公開APIに**XMLドキュメントコメント**を記述する

---

## 命名規則

### 一覧

| 対象             | スタイル    | 例                    |
| ---------------- | ----------- | --------------------- |
| 名前空間         | PascalCase  | `VehicleVision.Tools` |
| クラス           | PascalCase  | `SvgRenderer`         |
| インターフェース | IPascalCase | `IRenderer`           |
| メソッド         | PascalCase  | `RenderElement`       |
| プロパティ       | PascalCase  | `ScreenTitle`         |
| パブリック定数   | PascalCase  | `DefaultTheme`        |
| プライベート変数 | \_camelCase | `_renderer`           |
| ローカル変数     | camelCase   | `elementCount`        |
| パラメータ       | camelCase   | `inputPath`           |
| 非同期メソッド   | PascalCase  | `GenerateAsync`       |

### 詳細ルール

- **bool型**: `Is`, `Has`, `Can` プレフィックスを使用（例: `IsVisible`, `HasChildren`）
- **コレクション**: 複数形を使用（例: `Elements`, `Controls`）
- **略語**: 2文字は大文字（例: `IO`）、3文字以上はPascalCase（例: `Svg`, `Xml`）

---

## フォーマット

### インデントと空白

- **インデント**: スペース4つ
- **行末の空白**: 削除
- **ファイル末尾**: 改行を挿入

### 中括弧

Allmanスタイル（新しい行に配置）：

```csharp
if (condition)
{
    DoSomething();
}
else
{
    DoSomethingElse();
}
```

---

## コードスタイル

### var の使用

- 型が明らかな場合は `var` を使用
- 型が不明瞭な場合は明示的な型を使用

```csharp
// OK: 型が明らか
var renderer = new SvgRenderer();
var elements = new List<Element>();

// OK: 型を明示
ScreenDefinition definition = ParseYaml(content);
```

### パターンマッチング

- `is null` / `is not null` を使用（`== null` / `!= null` より推奨）

```csharp
if (element is not null)
{
    // ...
}
```

### null チェック

- null合体演算子を活用

```csharp
var title = screen.Title ?? "無題";
```

### 文字列

- 文字列補間 `$""` を使用（`string.Format` より推奨）
- 複数行は raw string literal を使用

---

## コメントとドキュメント

### XMLドキュメントコメント

公開APIには必ずXMLドキュメントコメントを記述する：

```csharp
/// <summary>
/// YAML定義からSVG画像を生成する。
/// </summary>
/// <param name="definition">画面定義。</param>
/// <returns>生成されたSVG文字列。</returns>
public string Render(ScreenDefinition definition)
{
    // ...
}
```

---

## エラーハンドリング

- 空のcatchブロックは避ける
- 例外は具体的な型でキャッチ
- `throw;` で再スロー（`throw ex;` は使用しない）

```csharp
try
{
    var content = File.ReadAllText(path);
}
catch (FileNotFoundException ex)
{
    Console.Error.WriteLine($"ファイルが見つかりません: {ex.FileName}");
    throw;
}
```

---

## ファイル構成

### ディレクトリ構造

```text
VehicleVision.Tools.ScreenSketch/
├── Generation/     # Markdown生成ロジック
├── Models/         # YAML定義モデル
├── Rendering/      # SVGレンダリング・テーマ
└── Samples/        # サンプルYAMLファイル
```

### usingディレクティブ

- `System` 系を先頭に配置
- グループ間に空行は入れない
- 不要な `using` は削除

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using VehicleVision.Tools.ScreenSketch.Models;
```

### ファイル内の順序

```csharp
// 1. usingディレクティブ
// 2. 名前空間
// 3. クラス定義
//    4. 定数・静的フィールド
//    5. フィールド
//    6. コンストラクタ
//    7. プロパティ
//    8. 公開メソッド
//    9. 内部メソッド
//   10. プライベートメソッド
```

---

## ツール設定

### EditorConfig

プロジェクトルートの `.editorconfig` でコードスタイルを統一している。詳細は `.editorconfig` ファイルを参照。

### Directory.Build.props

プロジェクト全体の共通設定は `Directory.Build.props` で管理：

```xml
<Project>
  <PropertyGroup>
    <LangVersion>latest</LangVersion>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <AnalysisLevel>latest-recommended</AnalysisLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors Condition="'$(Configuration)' == 'Release'">true</TreatWarningsAsErrors>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);CS1591</NoWarn>
    <WarningLevel>5</WarningLevel>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

---

## 参考リンク

- [C# コーディング規則](https://learn.microsoft.com/ja-jp/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [.NET コードスタイル規則](https://learn.microsoft.com/ja-jp/dotnet/fundamentals/code-analysis/style-rules/)
- [EditorConfig](https://editorconfig.org/)
