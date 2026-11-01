# vericairn-example-app

> **A demonstration, under construction for the Vericairn 0.9.0 launch.** This is not a customer product.

`tally` counts what text holds. It is the tiny example product that [Vericairn](https://github.com/Vericairn/vericairn)'s public example estate, [vericairn-example-estate](https://github.com/Vericairn/vericairn-example-estate), manages end to end: each change after this starting point comes from a work order in that estate, carried by an agent's recorded run, reviewed by another principal and accepted before it ships.

```sh
tally notes.txt          # the number of lines in a file
printf 'one\ntwo\n' | tally   # or in standard input
```

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
