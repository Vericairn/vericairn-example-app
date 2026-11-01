// SPDX-License-Identifier: Apache-2.0
using Tally;

// tally [file...]: prints the number of lines in each file, or in standard input when no file is named.
if (args.Length == 0)
{
    Console.WriteLine(Counter.Lines(Console.In.ReadToEnd()));
    return 0;
}

int exit = 0;
foreach (string path in args)
{
    try
    {
        Console.WriteLine($"{Counter.Lines(File.ReadAllText(path))} {path}");
    }
    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
    {
        Console.Error.WriteLine($"tally: {path}: {exception.Message}");
        exit = 1;
    }
}

return exit;
