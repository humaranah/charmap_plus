using System.Globalization;

namespace CharMapPlus.Core.Models;

public record GlyphInfo(
    string Character,
    string Name,
    int CodePoint,
    UnicodeCategory Category,
    string FontFamilyName
);