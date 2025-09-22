using CharMapPlus.Infrastructure.Abstractions;
using CharMapPlus.Infrastructure.DirectWrite;
using System.Globalization;

namespace CharMapPlus.Infrastructure;

public class DWriteFontCollectionProvider(IDwFontFactoryWrapper factory) : IFontCollectionProvider
{
    private IDwFontCollectionWrapper? _fontCollection;

    public IDwFontCollectionWrapper FontCollection => _fontCollection ??= factory.GetSystemFontCollection(false);

    public ICollection<IDwFontFamilyWrapper> GetFontFamilies()
    {
        uint count = FontCollection.FontFamilyCount;
        var results = new IDwFontFamilyWrapper[count];
        for (uint i = 0; i < count; i++)
            results[i] = FontCollection.GetFontFamily(i);
        return results;
    }

    public IDwFontFamilyWrapper GetFontFamily(uint index)
        => FontCollection.GetFontFamily(index);

    public string GetLocalizedFaceName(IDwFontWrapper font, string? cultureName = null)
    {
        var faceNames = font.FaceNames;
        cultureName ??= CultureInfo.CurrentCulture.Name;
        for (uint i = 0; i < faceNames.Count; i++)
        {
            if (faceNames.GetLocaleName(i).Equals(cultureName, StringComparison.OrdinalIgnoreCase))
            {
                return faceNames.GetString(i);
            }
        }
        return faceNames.GetString(0);
    }
}
