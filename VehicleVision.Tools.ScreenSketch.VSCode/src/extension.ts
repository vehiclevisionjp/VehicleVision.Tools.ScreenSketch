import * as vscode from "vscode";
import type MarkdownIt from "markdown-it";
import * as yaml from "js-yaml";
import { ScreenDefinition } from "./models";
import { ThemeColors } from "./themeColors";
import { SvgRenderer } from "./svgRenderer";

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
            const yamlContent = token.content;
            const svg = renderSync(yamlContent);
            if (svg !== null) {
                return `<div class="screen-sketch-preview">${svg}</div>`;
            }
            return (
                `<div class="screen-sketch-error" style="color:#c00;font-size:12px;margin-bottom:8px;">` +
                `⚠ screen-sketch render failed</div>` +
                defaultFence(tokens, idx, options, env, self)
            );
        }

        return defaultFence(tokens, idx, options, env, self);
    };
}

function renderSync(yamlContent: string): string | null {
    try {
        const config = vscode.workspace.getConfiguration("screenSketch");
        const theme = config.get<string>("theme", "");

        const definition = yaml.load(yamlContent) as ScreenDefinition;
        const colors = ThemeColors.fromName(
            theme || definition?.screen?.theme,
            definition?.screen?.customTheme,
        );
        const renderer = new SvgRenderer(colors);
        return renderer.render(definition ?? {});
    } catch {
        return null;
    }
}

export function deactivate() {}
