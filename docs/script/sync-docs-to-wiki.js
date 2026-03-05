// @ts-check
/// <reference types="node" />

/**
 * docs/wiki/ の内容を GitHub Wiki リポジトリへ同期するスクリプト。
 * 画像パスの相対参照をリポジトリの raw URL に書き換える。
 *
 * 使い方:
 *   node docs/script/sync-docs-to-wiki.js <wiki-dir> <repo-owner> <repo-name> <branch>
 *
 * 例:
 *   node docs/script/sync-docs-to-wiki.js ./wiki-temp vehiclevisionjp VehicleVision.Tools.ScreenSketch main
 */

const fs = require("fs");
const path = require("path");

const [, , wikiDir, repoOwner, repoName, branch = "main"] = process.argv;

if (!wikiDir || !repoOwner || !repoName) {
    console.error(
        "Usage: node sync-docs-to-wiki.js <wiki-dir> <repo-owner> <repo-name> [branch]",
    );
    process.exit(1);
}

const docsWikiDir = path.resolve(__dirname, "..", "wiki");
const outputDir = path.resolve(wikiDir);
const rawBase = `https://raw.githubusercontent.com/${repoOwner}/${repoName}/${branch}`;

if (!fs.existsSync(docsWikiDir)) {
    console.error(`docs/wiki directory not found: ${docsWikiDir}`);
    process.exit(1);
}

fs.mkdirSync(outputDir, { recursive: true });

const files = fs.readdirSync(docsWikiDir).filter((f) => f.endsWith(".md"));

for (const file of files) {
    let content = fs.readFileSync(path.join(docsWikiDir, file), "utf-8");

    // ../../images/xxx.svg → raw GitHub URL
    content = content.replace(
        /\(\.\.\/\.\.\/images\//g,
        `(${rawBase}/images/`,
    );

    fs.writeFileSync(path.join(outputDir, file), content, "utf-8");
    console.log(`Synced: ${file}`);
}

console.log(`Done. ${files.length} files synced to ${outputDir}`);
