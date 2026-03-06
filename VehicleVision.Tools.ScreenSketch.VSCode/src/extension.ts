import * as vscode from "vscode";
import { execFileSync } from "child_process";
import type MarkdownIt from "markdown-it";

export function activate(_context: vscode.ExtensionContext) {
    return {
        extendMarkdownIt(md: MarkdownIt) {
            return md.use(screenSketchPlugin);
        },
    };
}

function screenSketchPlugin(md: MarkdownIt): void {
    const defaultFence =
        md.renderer.rules.fence ||
        function (tokens, idx, options, _env, self) {
            return self.renderToken(tokens, idx, options);
        };

    md.renderer.rules.fence = (tokens, idx, options, env, self) => {
        const token = tokens[idx];

        if (token.info.trim() === "yaml-screen") {
            const yaml = token.content;
            const svg = renderSync(yaml);
            if (svg !== null) {
                return `<div class="screen-sketch-preview">${svg}</div>`;
            }
            // レンダリング失敗時はエラーメッセージ + 元のコードブロックを表示
            return (
                `<div class="screen-sketch-error" style="color:#c00;font-size:12px;margin-bottom:8px;">` +
                `⚠ screen-sketch render failed. Is the tool installed? ` +
                `(<code>dotnet tool install -g VehicleVision.Tools.ScreenSketch</code>)</div>` +
                defaultFence(tokens, idx, options, env, self)
            );
        }

        return defaultFence(tokens, idx, options, env, self);
    };
}

/**
 * screen-sketch render コマンドを同期的に呼び出し、SVG 文字列を返す。
 * プレビュー描画は同期コンテキストで実行されるため、child_process.execFileSync を使用。
 */
function renderSync(yaml: string): string | null {
    try {
        const config = vscode.workspace.getConfiguration("screenSketch");
        const toolPath = config.get<string>("toolPath", "screen-sketch");
        const theme = config.get<string>("theme", "");

        const args = ["render"];
        if (theme) {
            args.push("--theme", theme);
        }

        const result = execFileSync(toolPath, args, {
            input: yaml,
            encoding: "utf-8",
            timeout: 10_000,
            maxBuffer: 4 * 1024 * 1024,
            windowsHide: true,
        });

        return result;
    } catch {
        return null;
    }
}

export function deactivate() {}
