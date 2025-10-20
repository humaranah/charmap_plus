using CharMapPlus.Core.Abstrations;
using CharMapPlus.Core.Models;
using CharMapPlus.Core.Util;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Vortice.DirectWrite;

namespace CharMapPlus.Infrastructure.DirectWrite;

public class DwFont(uint id, IDWriteFont font) : IFont
{
    public uint Id => id;

    public ICollection<GlyphInfo> GetSupportedGlyphs()
    {
        var fontFace = font.CreateFontFace();
        if (fontFace.GlyphCount == 0)
            return [];
        var supportedChars = new List<GlyphInfo>();
        // Process code points in batches to optimize performance
        var codePoints = UnicodeRanges.ValidCodePoints;

        const int batchSize = 512;
        var batches = codePoints.Chunk(batchSize);

        foreach (var batch in batches)
        {
            var glyphIndices = fontFace.GetGlyphIndices(batch);
            for (int i = 0; i < batch.Length; i++)
            {
                if (TryCreateGlyph(batch[i], glyphIndices[i], out var glyph))
                {
                    supportedChars.Add(glyph);
                }
            }
        }

        return [.. supportedChars.OrderBy(g => g.CodePoint)];
    }

    public bool TryGetFullName([NotNullWhen(true)] out string? fullName) =>
        TryGetString(InformationalStringId.Win32FamilyNames, out fullName);

    private bool TryGetString(InformationalStringId id, [NotNullWhen(true)] out string? result)
    {
        font.GetInformationalStrings(id, out var strings, out var exists);
        if (!exists || strings is null)
        {
            result = null;
            return false;
        }
        result = strings.GetString(0);
        return true;
    }

    private bool TryCreateGlyph(uint codePoint, ushort index, [NotNullWhen(true)] out GlyphInfo? info)
    {
        if (index == 0)
        {
            info = null;
            return false;
        }
        var character = char.ConvertFromUtf32((int)codePoint);
        info = new GlyphInfo(
            character,
            codePoint,
            index,
            CharUnicodeInfo.GetUnicodeCategory(character, 0),
            TryGetFullName(out var name) ? name : "Unknown");
        return true;
    }
}
