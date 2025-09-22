using CharMapPlus.Core;
using CharMapPlus.Core.Models;
using CharMapPlus.Infrastructure.Abstractions;
using CharMapPlus.Infrastructure.Models;
using System.Globalization;

namespace CharMapPlus.Infrastructure;

public class FontService(IFontCollectionProvider provider) : IFontService
{
    // Unicode code points from U+0021 to U+D7FF and U+E000 to U+FFFF
    // Excludes control characters and surrogate pairs
    private static readonly uint[] CodePoints = [.. Enumerable
        .Range(0x0021, 0xD800 - 0x0021 + 1)
        .Concat(Enumerable.Range(0xE000, 0xFFFF - 0xE000 + 1))
        .Select(codePoint => (uint)codePoint)];

    private readonly Dictionary<string, FontMap> _fontMap = [];

    public IReadOnlyDictionary<string, FontMap> FontMap => _fontMap;

    public void LoadFonts()
    {
        _fontMap.Clear();
        var families = provider.GetFontFamilies() ?? [];
        if (families.Count == 0)
            return;

        foreach (var family in families)
        {
            var fonts = family.GetFonts();
            if (!fonts.Any())
                continue;

            foreach (var font in fonts)
            {
                if (font.TryGetFullName(out string? fullName) && !_fontMap.ContainsKey(fullName))
                {
                    _fontMap[fullName] = new FontMap(family.Index, font.Index);
                }
            }
        }
    }

    public ICollection<FontInfo> ListFonts()
    {
        var fonts = new List<FontInfo>();
        if (_fontMap.Count == 0)
            return fonts;
        foreach ((var key, var value) in _fontMap)
        {
            var fontFamily = provider.GetFontFamily(value.FontFamilyId);
            fonts.Add(new FontInfo(
                Name: key,
                FamilyName: fontFamily.GetFamilyName()
            ));
        }
        return [.. fonts.OrderBy(x => x.Name)];
    }

    public ICollection<GlyphInfo> GetFontSupportedGlyphs(string fontName)
    {
        var supportedChars = new List<GlyphInfo>();
        if (_fontMap.Count == 0 || !_fontMap.TryGetValue(fontName, out FontMap? fontMap))
            return supportedChars;

        var fontFamily = provider.GetFontFamily(fontMap.FontFamilyId);
        var font = fontFamily.GetFont(fontMap.FontId);
        var fontFace = font.CreateFontFace();

        if (fontFace.GlyphCount == 0)
            return supportedChars;

        // Process code points in batches to optimize performance
        const int batchSize = 512;
        var batches = CodePoints.Chunk(batchSize);
        foreach(var batch in batches)
        {
            var glyphIndices = fontFace.GetGlyphIndices(batch);
            for (int i = 0; i < batch.Length; i++)
            {
                var codePoint = batch[i];
                var glyphIndex = glyphIndices[i];
                if (glyphIndex != 0)
                {
                    var character = char.ConvertFromUtf32((int)codePoint);
                    supportedChars.Add(new GlyphInfo(
                        character,
                        codePoint,
                        glyphIndex,
                        CharUnicodeInfo.GetUnicodeCategory(character, 0),
                        fontName));
                }
            }
        }

        return [.. supportedChars.OrderBy(g => g.CodePoint)];
    }
}
