namespace CharMapPlus.Core.Util;

public static class UnicodeRanges
{
    /// <summary>
    /// Gets unicode code points from U+0021 to U+D7FF and U+E000 to U+FFFF
    /// </summary>
    /// <remarks>
    /// Excludes control characters and surrogate pairs
    /// </remarks>
    public static IEnumerable<uint> ValidCodePoints { get; } = Enumerable
        .Range(0x0021, 0xD800 - 0x0021 + 1)
        .Concat(Enumerable.Range(0xE000, 0xFFFF - 0xE000 + 1))
        .Select(codePoint => (uint)codePoint);
}
