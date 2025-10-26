using CharMapPlus.Core.Abstrations;
using CharMapPlus.Core.Models;
using Microsoft.Extensions.Logging;

namespace CharMapPlus.Infrastructure;

public class FontService(
    IFontCollectionProvider provider,
    ILogger<FontService> logger) : IFontService
{
    private readonly Dictionary<string, FontMap> _fontMap = [];
    public IReadOnlyDictionary<string, FontMap> FontMap => _fontMap;

    public Task LoadFontsAsync()
    {
        return Task.Run(() =>
        {
            _fontMap.Clear();
            logger.LogInformation("Initializing font mapping...");
            foreach (var family in provider.GetFontFamilies())
            {
                foreach (var font in family.GetFonts())
                {
                    if (font.TryGetFullName(out string? fullName))
                    {
                        _fontMap.TryAdd(fullName, new FontMap(family.Id, font.Id));
                    }
                }
            }
            logger.LogInformation("Mapped {Count} fonts", _fontMap.Count);
        });
    }

    public ICollection<FontInfo> ListFonts()
    {
        var fonts = new List<FontInfo>();
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
        if (_fontMap.Count == 0 || !_fontMap.TryGetValue(fontName, out FontMap? fontMap))
            return [];

        var fontFamily = provider.GetFontFamily(fontMap.FontFamilyId);
        if (!fontFamily.TryGetFont(fontMap.FontId, out var font))
            return [];

        var glyphs = font.GetSupportedGlyphs();
        if (glyphs.Count == 0)
            return [];

        logger.LogDebug("Found {Count} glyphs for font {FontName}", glyphs.Count, fontName);
        return glyphs;
    }
}
