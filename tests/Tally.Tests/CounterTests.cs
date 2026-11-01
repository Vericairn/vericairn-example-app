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
    public void LinesCountsBreaksAndAFinalUnterminatedLine(string text, long lines) => Assert.Equal(lines, Counter.Lines(text));
}
