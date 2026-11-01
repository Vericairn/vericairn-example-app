// SPDX-License-Identifier: Apache-2.0
using System.Text;
using System.Text.Json;
using Tally;

// tally [--json] [file...]: prints counts for each file, or for standard input.
bool json = args.Contains("--json");
string[] paths = args.Where(argument => argument != "--json").ToArray();
if (paths.Length == 0)
{
    using var input = new StreamReader(Console.OpenStandardInput(), Encoding.UTF8);
    string text = input.ReadToEnd();
    PrintCounts(text, null, json);
    return 0;
}

int exit = 0;
foreach (string path in paths)
{
    try
    {
        string text = File.ReadAllText(path);
        PrintCounts(text, path, json);
    }
    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
    {
        Console.Error.WriteLine($"tally: {path}: {exception.Message}");
        exit = 1;
    }
}

return exit;

static void PrintCounts(string text, string? path, bool json)
{
    long lines = Counter.Lines(text);
    long words = Counter.Words(text);
    long characters = Counter.Characters(text);
    if (json)
    {
        Console.WriteLine(JsonSerializer.Serialize(new { path, lines, words, characters }));
    }
    else
    {
        Console.WriteLine(path is null
            ? $"{lines} {words} {characters}"
            : $"{lines} {words} {characters} {path}");
    }
}
