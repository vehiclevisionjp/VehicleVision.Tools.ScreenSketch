# サンプル集

VehicleVision.Tools.ScreenSketch の画面定義サンプルと出力例です。

## 基本的な画面

```yaml
title: ログイン画面
theme: default
width: 400
height: 300
controls:
  - type: label
    text: ユーザー名
    x: 30
    y: 50
    width: 80
    height: 20
  - type: textbox
    text: ""
    x: 120
    y: 48
    width: 200
    height: 25
  - type: label
    text: パスワード
    x: 30
    y: 90
    width: 80
    height: 20
  - type: textbox
    text: ""
    x: 120
    y: 88
    width: 200
    height: 25
  - type: button
    text: ログイン
    x: 150
    y: 140
    width: 100
    height: 30
```

## テーマの使用例

### ダークテーマ

```yaml
title: ダッシュボード
theme: dark
width: 800
height: 600
controls:
  - type: menubar
    text: ファイル,編集,表示,ヘルプ
    x: 0
    y: 0
    width: 800
    height: 25
```

### ブループリントテーマ

```yaml
title: 設計画面
theme: blueprint
width: 600
height: 400
controls:
  - type: group
    text: 入力エリア
    x: 10
    y: 10
    width: 580
    height: 200
```

## 関連ページ

- [コマンドリファレンス](Command-Reference)
- [YAML定義リファレンス](YAML-Definition-Reference)
- [コントロール一覧](Controls)
- [Home](Home)
