namespace CharMapPlus.Core.Abstrations;

public interface IFontCollectionProvider
{
    IEnumerable<IFontFamily> GetFontFamilies();
    IFontFamily GetFontFamily(uint id);
}
