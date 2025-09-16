using CharMapPlus.Core;
using CharMapPlus.Core.Models;
using System.Drawing.Text;
using System.Globalization;
using Vortice.DirectWrite;

namespace CharMapPlus.Infrastructure;

public class FontService : IFontService
{
    private readonly IDWriteFontCollection? _fontCollection;

    public FontService()
    {
        var factory = DWrite.DWriteCreateFactory<IDWriteFactory>();
        _fontCollection = factory.GetSystemFontCollection(false);
    }

    public ICollection<string> GetAllFonts()
    {
        var fontsCollection = new InstalledFontCollection();
        return [.. fontsCollection.Families.Select(f => string.Intern(f.Name))];
    }

    public ICollection<GlyphInfo> GetFontSupportedCharacters(string fontName)
    {
        var supportedChars = new List<GlyphInfo>();

        if (_fontCollection is null)
            return supportedChars;

        if (!_fontCollection.FindFamilyName(fontName, out uint index))
        {
            // Need to infer the font family name and style from the provided name
            return supportedChars;
        }

        var fontFamily = _fontCollection.GetFontFamily(index);
        var font = fontFamily.GetFirstMatchingFont(
            FontWeight.Normal, FontStretch.Normal, FontStyle.Normal);

        var fontFace = font.CreateFontFace();

        for (int codePoint = 0x0021; codePoint <= 0xFFFF; codePoint++)
        {
            if (codePoint >= 0xD800 && codePoint <= 0xDFFF)
                continue; // Skip surrogate pairs


            var glyphIndices = new ushort[1];
            fontFace.GetGlyphIndices(new uint[] { (uint)codePoint }, glyphIndices);
            if (glyphIndices.Length > 0 && glyphIndices[0] != 0)
            {
                var character = char.ConvertFromUtf32(codePoint);
                var category = CharUnicodeInfo.GetUnicodeCategory(character, 0);
                supportedChars.Add(new GlyphInfo(
                    Character: character,
                    Name: string.Empty,
                    CodePoint: codePoint,
                    Category: category,
                    FontFamilyName: fontName
                ));
            }

        }
        return supportedChars;
    }
}
