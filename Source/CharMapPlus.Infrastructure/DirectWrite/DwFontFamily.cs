using CharMapPlus.Core.Abstrations;
using System.Diagnostics.CodeAnalysis;
using Vortice.DirectWrite;

namespace CharMapPlus.Infrastructure.DirectWrite;

public class DwFontFamily(uint fontId, IDWriteFontFamily fontFamily) : IFontFamily
{
    public uint Id => fontId;

    public string GetFamilyName() =>
        fontFamily.FamilyNames.GetString(0);

    public IEnumerable<IFont> GetFonts()
    {
        for (uint i = 0; i < fontFamily.FontCount; i++)
        {
            yield return new DwFont(i, fontFamily.GetFont(i));
        }
    }

    public bool TryGetFont(uint fontId, [NotNullWhen(true)] out IFont? font)
    {
        var fontCount = fontFamily.FontCount;
        if (fontCount == 0 || fontId >= fontCount)
        {
            font = null;
            return false;
        }
        var dwFont = fontFamily.GetFont(fontId);
        font = new DwFont(fontId, dwFont);
        return true;
    }
}
