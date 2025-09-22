using System.Globalization;

namespace CharMapPlus.Core.Models;

public record GlyphInfo(
    string Character,
    uint CodePoint,
    int GlyphIndex,
    UnicodeCategory Category,
    string FontName
);