// SPDX-License-Identifier: Apache-2.0
namespace Tally.Tests;

public sealed class CounterTests
{
    [Theory]
    [InlineData("", 0)]
    [InlineData("one", 1)]
    [InlineData("one\n", 1)]
    [InlineData("one\ntwo", 2)]
    [InlineData("one\ntwo\n", 2)]
    [InlineData("\n\n", 2)]
    [InlineData("one\r\ntwo\r\n", 2)]
    [InlineData("one\r\ntwo", 2)]
    [InlineData("café\n世界 \U0001F600", 2)]
    public void LinesCountsBreaksAndAFinalUnterminatedLine(string text, long lines) => Assert.Equal(lines, Counter.Lines(text));

    [Theory]
    [InlineData("", 0)]
    [InlineData(" \t\r\n\v\f\u0085\u00a0\u1680\u2003\u2028\u2029\u202f\u205f\u3000", 0)]
    [InlineData("one", 1)]
    [InlineData("one\n", 1)]
    [InlineData("one\ntwo", 2)]
    [InlineData("one\ntwo\n", 2)]
    [InlineData("  one\t\ttwo\r\nthree  ", 3)]
    [InlineData("one,two!", 1)]
    [InlineData("one\u200btwo", 1)]
    [InlineData("café\u00a0世界\u2003\U0001F600", 3)]
    [InlineData("one\u0085two\u2028three\u2029four", 4)]
    public void WordsCountsRunsOfNonWhitespace(string text, long words) => Assert.Equal(words, Counter.Words(text));

    [Theory]
    [InlineData("", 0)]
    [InlineData("one", 3)]
    [InlineData("one\n", 4)]
    [InlineData("one\ntwo", 7)]
    [InlineData("one\r\ntwo\r\n", 10)]
    [InlineData(" \t\r\n", 4)]
    [InlineData("café", 4)]
    [InlineData("世界", 2)]
    [InlineData("\U0001F600", 1)]
    [InlineData("a\U00010437b", 3)]
    [InlineData("e\u0301", 2)]
    [InlineData("café\u00a0世界 \U0001F600\n", 10)]
    public void CharactersCountsUnicodeScalars(string text, long characters) => Assert.Equal(characters, Counter.Characters(text));

    [Fact]
    public void CountsRejectNullInput()
    {
        Assert.Throws<ArgumentNullException>(() => Counter.Lines(null!));
        Assert.Throws<ArgumentNullException>(() => Counter.Words(null!));
        Assert.Throws<ArgumentNullException>(() => Counter.Characters(null!));
    }
}
