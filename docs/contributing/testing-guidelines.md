# テストガイドライン

このドキュメントでは、VehicleVision.Tools.ScreenSketch プロジェクトのテスト規約について説明します。

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

---

## 基本情報

### テストプロジェクト構成

| 項目                     | 内容                                     |
| ------------------------ | ---------------------------------------- |
| プロジェクト名           | `VehicleVision.Tools.ScreenSketch.Tests` |
| テストフレームワーク     | xUnit                                    |
| ターゲットフレームワーク | .NET 10                                  |
| カバレッジツール         | coverlet.collector                       |

### ディレクトリ構造

<!-- TODO: テストプロジェクト作成後に更新 -->

```text
VehicleVision.Tools.ScreenSketch.Tests/
├── VehicleVision.Tools.ScreenSketch.Tests.csproj
└── (テストファイル)
```

---

## テスト実行

### コマンドライン

```bash
# 全テストを実行
dotnet test

# 詳細なログを出力
dotnet test --logger "console;verbosity=detailed"

# カバレッジを収集
dotnet test --collect:"XPlat Code Coverage"
```

### Visual Studio / VS Code

- テストエクスプローラーからテストを実行・デバッグ可能
- C# Dev Kit 拡張機能を使用

---

## カバレッジ

- `coverlet.collector` を使用してカバレッジを収集
- カバレッジレポートは `TestResults/` ディレクトリに出力される

---

## テストの書き方

### テスト命名規則

`メソッド名_条件_期待結果` の形式を使用：

```csharp
[Fact]
public void Render_WithValidDefinition_ReturnsSvgString()
{
    // ...
}

[Fact]
public void Parse_WithEmptyYaml_ThrowsArgumentException()
{
    // ...
}
```

### テスト構造（AAA パターン）

```csharp
[Fact]
public void Render_WithButton_IncludesButtonElement()
{
    // Arrange
    var definition = new ScreenDefinition
    {
        Title = "テスト画面"
    };

    // Act
    var result = renderer.Render(definition);

    // Assert
    Assert.Contains("<rect", result);
}
```

---

## ベストプラクティス

- テストは独立して実行可能にする
- テストデータはテスト内で定義する（外部ファイル依存を避ける）
- 1テスト1アサーションを基本とする
- テスト名は日本語でもよいが、メソッド名形式を推奨
