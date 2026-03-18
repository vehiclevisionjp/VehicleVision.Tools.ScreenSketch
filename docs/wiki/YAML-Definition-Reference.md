# YAML定義リファレンス

画面定義 YAML ファイルのスキーマと全プロパティの解説です。

## 基本構造

```yaml
title: 画面タイトル
theme: default
width: 800
height: 600
controls:
  - type: label
    text: サンプル
    x: 10
    y: 10
    width: 100
    height: 20
```

## トップレベルプロパティ

| プロパティ | 型 | 必須 | 説明 |
| ---------- | -- | ---- | ---- |
| `title` | string | ○ | 画面タイトル |
| `theme` | string | - | テーマ名（default, dark, blueprint, custom） |
| `width` | number | - | 画面幅（px） |
| `height` | number | - | 画面高さ（px） |
| `controls` | array | ○ | コントロール定義の配列 |
| `annotations` | array | - | アノテーション定義の配列 |
| `connectors` | array | - | コネクタ定義の配列 |

## コントロール共通プロパティ

| プロパティ | 型 | 必須 | 説明 |
| ---------- | -- | ---- | ---- |
| `type` | string | ○ | コントロール種別 |
| `text` | string | - | 表示テキスト |
| `x` | number | ○ | X座標 |
| `y` | number | ○ | Y座標 |
| `width` | number | ○ | 幅 |
| `height` | number | ○ | 高さ |

各コントロール固有のプロパティについては[コントロール一覧](Controls)を参照してください。

## テーマ

| テーマ名 | 説明 |
| -------- | ---- |
| `default` | 標準の Windows フォーム風テーマ |
| `dark` | ダークモードテーマ |
| `blueprint` | 青写真風テーマ |
| `custom` | カスタムカラー定義 |

## 関連ページ

- [コントロール一覧](Controls)
- [サンプル集](Examples)
- [コマンドリファレンス](Command-Reference)
- [Home](Home)
