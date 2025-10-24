using CharMapPlus.Core.Abstrations;
using CharMapPlus.Core.Models;

namespace CharMapPlus.Infrastructure;

public class FontService(IFontCollectionProvider provider) : IFontService
{
    private readonly Dictionary<string, FontMap> _fontMap = [];
    public IReadOnlyDictionary<string, FontMap> FontMap => _fontMap;

    public Task LoadFontsAsync()
    {
        return Task.Run(() =>
        {
            _fontMap.Clear();
            foreach (var family in provider.GetFontFamilies())
            {
                foreach (var font in family.GetFonts())
                {
                    if (font.TryGetFullName(out string? fullName) && !_fontMap.ContainsKey(fullName))
                    {
                        _fontMap[fullName] = new FontMap(family.Id, font.Id);
                    }
                }
            }
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

        return glyphs;
    }
}
