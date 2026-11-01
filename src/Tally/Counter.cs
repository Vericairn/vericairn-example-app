// SPDX-License-Identifier: Apache-2.0
namespace Tally;

/// <summary>Counts what a text holds.</summary>
public static class Counter
{
    /// <summary>The number of lines: line breaks, plus one when the text ends without one.</summary>
    public static long Lines(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (text.Length == 0) return 0;
        long breaks = text.Count(c => c == '\n');
        return text.EndsWith('\n') ? breaks : breaks + 1;
    }
}
