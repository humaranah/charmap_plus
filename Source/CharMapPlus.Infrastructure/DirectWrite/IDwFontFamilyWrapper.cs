namespace CharMapPlus.Infrastructure.DirectWrite;

public interface IDwFontFamilyWrapper
{
    uint Index { get; }
    uint FontCount { get; }
    string GetFamilyName();
    IEnumerable<IDwFontWrapper> GetFonts();
    IDwFontWrapper GetFont(uint index);
}
