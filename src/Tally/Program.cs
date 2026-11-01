// SPDX-License-Identifier: Apache-2.0
using Tally;

// tally [file...]: prints lines, words and characters for each file, or for standard input.
if (args.Length == 0)
{
    string text = Console.In.ReadToEnd();
    Console.WriteLine($"{Counter.Lines(text)} {Counter.Words(text)} {Counter.Characters(text)}");
    return 0;
}

int exit = 0;
foreach (string path in args)
{
    try
    {
        string text = File.ReadAllText(path);
        Console.WriteLine($"{Counter.Lines(text)} {Counter.Words(text)} {Counter.Characters(text)} {path}");
    }
    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
    {
        Console.Error.WriteLine($"tally: {path}: {exception.Message}");
        exit = 1;
    }
}

return exit;
