# VS Code 拡張機能

VehicleVision.Tools.ScreenSketch の VS Code 拡張機能について説明します。

## 概要

VS Code 上で YAML 画面定義ファイルをリアルタイムプレビューできる拡張機能です。

## インストール

VS Code Marketplace からインストールできます。

```text
拡張機能パネル → "VehicleVision ScreenSketch" を検索 → インストール
```

## 機能

- YAML ファイルのリアルタイム SVG プレビュー
- シンタックスハイライト
- コード補完

## 使い方

1. `.yaml` ファイルを開く
2. コマンドパレット（`Ctrl+Shift+P`）を開く
3. `ScreenSketch: Preview` を実行

## 技術仕様

拡張機能は TypeScript で実装された内蔵 SVG レンダラーを使用します。
外部ツールのインストールは不要です。

## 関連ページ

- [コマンドリファレンス](Command-Reference)
- [YAML定義リファレンス](YAML-Definition-Reference)
- [Home](Home)
