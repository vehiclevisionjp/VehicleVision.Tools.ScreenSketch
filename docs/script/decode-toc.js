/**
 * doctocが生成したTOCを後処理するスクリプト
 * - URLエンコードされた日本語をデコード
 * - H1へのリンク（ファイルのH1タイトルと一致する項目）を削除
 * - 中黒（・）をアンカーから削除（GitHub仕様に準拠）
 * - インデントを2スペースから4スペースに変換（MD007対応）
 *
 * 使用方法:
 *   node docs/script/decode-toc.js              # 全ファイルを処理
 *   node docs/script/decode-toc.js path/to.md   # 指定ファイルのみ処理
 */

const fs = require("fs");
const path = require("path");

const docsDir = path.join(__dirname, "..");
const rootDir = path.join(__dirname, "..", "..");

/**
 * 指定ディレクトリ内の.mdファイルを再帰的に収集する
 * @param {string} dir - 探索するディレクトリパス
 * @param {string[]} [excludeDirs=[]] - 除外するディレクトリ名
 * @returns {string[]} .mdファイルの絶対パス一覧
 */
function collectMdFiles(dir, excludeDirs = []) {
    const results = [];
    const entries = fs.readdirSync(dir, { withFileTypes: true });
    for (const entry of entries) {
        const fullPath = path.join(dir, entry.name);
        if (entry.isDirectory()) {
            if (!excludeDirs.includes(entry.name)) {
                results.push(...collectMdFiles(fullPath));
            }
        } else if (entry.isFile() && entry.name.endsWith(".md")) {
            results.push(fullPath);
        }
    }
    return results;
}

// コマンドライン引数で単一ファイルが指定された場合はそのファイルのみ処理
const targetFile = process.argv[2];

let files;
if (targetFile) {
    const resolved = path.resolve(targetFile);
    if (!fs.existsSync(resolved)) {
        console.error(`File not found: ${resolved}`);
        process.exit(1);
    }
    files = [resolved];
} else {
    // docs配下の全.mdファイルを再帰的に収集（scriptディレクトリは除外）
    files = collectMdFiles(docsDir, ["script"]);

    // ルートディレクトリのmdファイルも追加
    fs.readdirSync(rootDir)
        .filter((f) => f.endsWith(".md"))
        .forEach((f) => files.push(path.join(rootDir, f)));
}

files.forEach((filePath) => {
    let content = fs.readFileSync(filePath, "utf8");

    // ファイルのH1タイトルを取得
    const h1Match = content.match(/^# (.+)$/m);
    const h1Title = h1Match ? h1Match[1].trim() : null;

    // doctocマーカー内を処理
    const doctocRegex =
        /(<!-- START doctoc[\s\S]*?)(- \[[\s\S]*?)(<!-- END doctoc[\s\S]*?-->)/g;

    const newContent = content.replace(doctocRegex, (match, start, toc, end) => {
        let processed = toc;

        // 1. URLエンコードされた日本語をデコード
        processed = processed.replace(
            /\(#([^)]+)\)/g,
            (m, anchor) => `(#${decodeURIComponent(anchor)})`
        );

        // 2. H1タイトルと一致するリンクを削除
        if (h1Title) {
            const escapedTitle = h1Title.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
            const h1LinkRegex = new RegExp(
                `^- \\[${escapedTitle}\\]\\(#[^)]+\\)\\n`,
                "m"
            );
            processed = processed.replace(h1LinkRegex, "");
        }

        // 3. 中黒（・）をアンカーから削除（GitHub仕様に準拠）
        processed = processed.replace(
            /\(#([^)]*)\)/g,
            (m, anchor) => `(#${anchor.replace(/・/g, "")})`
        );

        // 4. インデントを2スペースから4スペースに変換（MD007対応）
        processed = processed.replace(/^( +)-/gm, (m, spaces) => {
            const depth = spaces.length / 2;
            return " ".repeat(depth * 4) + "-";
        });

        return start + processed + end;
    });

    if (newContent !== content) {
        fs.writeFileSync(filePath, newContent, "utf8");
    }
});
