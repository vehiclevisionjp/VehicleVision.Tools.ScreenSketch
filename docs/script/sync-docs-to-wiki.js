// @ts-check
/// <reference types="node" />

/**
 * docs/wiki/ の内容を GitHub Wiki リポジトリへ同期するスクリプト。
 * Markdown ファイルと images/ フォルダをそのままコピーする。
 *
 * 使い方:
 *   node docs/script/sync-docs-to-wiki.js <wiki-dir>
 *
 * 例:
 *   node docs/script/sync-docs-to-wiki.js ./wiki-temp
 */

const fs = require("fs");
const path = require("path");

const [, , wikiDir] = process.argv;

if (!wikiDir) {
    console.error("Usage: node sync-docs-to-wiki.js <wiki-dir>");
    process.exit(1);
}

const docsWikiDir = path.resolve(__dirname, "..", "wiki");
const outputDir = path.resolve(wikiDir);

if (!fs.existsSync(docsWikiDir)) {
    console.error(`docs/wiki directory not found: ${docsWikiDir}`);
    process.exit(1);
}

fs.mkdirSync(outputDir, { recursive: true });

// Markdown ファイルをコピー
const mdFiles = fs.readdirSync(docsWikiDir).filter((f) => f.endsWith(".md"));

for (const file of mdFiles) {
    fs.copyFileSync(path.join(docsWikiDir, file), path.join(outputDir, file));
    console.log(`Synced: ${file}`);
}

// images/ フォルダをコピー
const srcImagesDir = path.join(docsWikiDir, "images");
const destImagesDir = path.join(outputDir, "images");

if (fs.existsSync(srcImagesDir)) {
    fs.mkdirSync(destImagesDir, { recursive: true });
    const imageFiles = fs.readdirSync(srcImagesDir);
    for (const file of imageFiles) {
        fs.copyFileSync(
            path.join(srcImagesDir, file),
            path.join(destImagesDir, file),
        );
    }
    console.log(`Synced: ${imageFiles.length} image(s)`);
}

console.log(
    `Done. ${mdFiles.length} markdown files synced to ${outputDir}`,
);
