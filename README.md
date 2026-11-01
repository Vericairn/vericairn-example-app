# vericairn-example-app

> **A demonstration, under construction for the Vericairn 0.9.0 launch.** This is not a customer product.

`tally` counts what text holds. It is the tiny example product that [Vericairn](https://github.com/Vericairn/vericairn)'s public example estate, [vericairn-example-estate](https://github.com/Vericairn/vericairn-example-estate), manages end to end: each change after this starting point comes from a work order in that estate, carried by an agent's recorded run, reviewed by another principal and accepted before it ships.

```sh
printf 'one two\nthree\n' > notes.txt
tally notes.txt
# 2 3 14 notes.txt

printf 'one two\nthree' | tally
# 2 3 13
```

By default, `tally` prints `lines words characters path` for each file, in argument order. With no file arguments, it reads standard input and prints `lines words characters`. The counts are separated by single spaces.

Files and standard input default to UTF-8 decoding on every platform, including Windows. A leading byte-order mark selects the encoding and is not counted as text.

- **Lines**: the number of line breaks (`\n`), plus one for a final line without a line break.
- **Words**: runs of non-whitespace characters. Spaces, tabs, line breaks and other Unicode whitespace separate words.
- **Characters**: Unicode scalar values, including whitespace and line breaks. For example, `😀` counts as one character; an `e` followed by a combining accent counts as two. A Windows line ending (`\r\n`) contributes two characters and one line break.

Empty input has zero lines, words and characters. An unreadable file produces a message on standard error and exit status 1; remaining files are still processed.

Use `--json` to print one JSON object per input, on its own line in argument order. Each object has `path`, `lines`, `words` and `characters`; the counts are numbers, and `path` is the supplied file path or `null` for standard input. The option can appear before or after file arguments.

```sh
tally --json notes.txt
# {"path":"notes.txt","lines":2,"words":3,"characters":14}

printf 'one two\nthree' | tally --json
# {"path":null,"lines":2,"words":3,"characters":13}
```

Errors still go to standard error with exit status 1. Unreadable files produce no JSON object, and the remaining files are still counted. Without `--json`, the output stays as shown above.

## Build and test

You need the .NET 10 SDK.

```sh
dotnet test Tally.slnx
dotnet run --project src/Tally -- README.md
```

## Releases

Releases are unsigned developer builds through GitHub Releases: one self-contained `tally` executable per platform (Linux x64, macOS arm64, Windows x64), with `SHA256SUMS`. The release workflow builds a draft from an exact commit when the example estate submits a release, and the draft is published only after the estate records the release's acceptance.

## License

Apache-2.0. See [LICENSE](LICENSE).
