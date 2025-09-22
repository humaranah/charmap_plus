namespace CharMapPlus.Infrastructure.DirectWrite;

public interface IDwFontCollectionWrapper
{
    uint FontFamilyCount { get; }
    IDwFontFamilyWrapper GetFontFamily(uint index);
}
