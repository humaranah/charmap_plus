using CharMapPlus.Core.Abstrations;
using Vortice.DirectWrite;

namespace CharMapPlus.Infrastructure.DirectWrite;

public class DwFontCollectionProvider(
    IDWriteFontCollectionFactory factory) : IFontCollectionProvider
{
    private readonly Lazy<IDWriteFontCollection> _fontCollection = new(factory.Create);

    public IEnumerable<IFontFamily> GetFontFamilies()
    {
        var fontCollection = _fontCollection.Value;
        for (uint i = 0; i < fontCollection.FontFamilyCount; i++)
        {
            var fontFamily = fontCollection.GetFontFamily(i);
            yield return new DwFontFamily(i, fontFamily);
        }
    }

    public IFontFamily GetFontFamily(uint id)
    {
        var fontCollection = _fontCollection.Value;
        var fontFamily = fontCollection.GetFontFamily(id);
        return new DwFontFamily(id, fontFamily);
    }
}
