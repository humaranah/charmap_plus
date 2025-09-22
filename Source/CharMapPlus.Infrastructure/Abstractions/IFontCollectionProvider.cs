using CharMapPlus.Infrastructure.DirectWrite;

namespace CharMapPlus.Infrastructure.Abstractions;

public interface IFontCollectionProvider
{
    IDwFontCollectionWrapper FontCollection { get; }

    ICollection<IDwFontFamilyWrapper> GetFontFamilies();

    IDwFontFamilyWrapper GetFontFamily(uint index);

    string GetLocalizedFaceName(IDwFontWrapper font, string? cultureName = null);
}
