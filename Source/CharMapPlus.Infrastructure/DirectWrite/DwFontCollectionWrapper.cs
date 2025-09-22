using Vortice.DirectWrite;

namespace CharMapPlus.Infrastructure.DirectWrite;

public class DwFontCollectionWrapper(IDWriteFontCollection fontCollection) : IDwFontCollectionWrapper
{
    public uint FontFamilyCount => fontCollection.FontFamilyCount;

    public IDwFontFamilyWrapper GetFontFamily(uint index)
    {
        var fontFamily = fontCollection.GetFontFamily(index);
        return new DwFontFamilyWrapper(index, fontFamily);
    }
}
