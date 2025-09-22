using Vortice.DirectWrite;

namespace CharMapPlus.Infrastructure.DirectWrite;

public class DwFontFaceWrapper(IDWriteFontFace fontFace) : IDwFontFaceWrapper
{
    public ushort GlyphCount => fontFace.GlyphCount;

    public ushort[] GetGlyphIndices(IEnumerable<uint> codePoints)
    {
        var codePointArray = codePoints as uint[] ?? [.. codePoints];
        var glyphIndices = new ushort[codePointArray.Length];
        _ = fontFace.GetGlyphIndices(codePointArray, glyphIndices);
        return glyphIndices;
    }
}
