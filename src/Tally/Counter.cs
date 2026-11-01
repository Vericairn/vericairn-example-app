// SPDX-License-Identifier: Apache-2.0
using System.Text;

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

    /// <summary>The number of runs of non-whitespace characters.</summary>
    public static long Words(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        long words = 0;
        bool inWord = false;
        foreach (Rune character in text.EnumerateRunes())
        {
            if (Rune.IsWhiteSpace(character))
            {
                inWord = false;
            }
            else if (!inWord)
            {
                words++;
                inWord = true;
            }
        }

        return words;
    }

    /// <summary>The number of Unicode scalar values, including whitespace.</summary>
    public static long Characters(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        long characters = 0;
        foreach (Rune character in text.EnumerateRunes())
        {
            characters++;
        }

        return characters;
    }
}
