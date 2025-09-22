namespace CharMapPlus.Infrastructure.DirectWrite;

public interface IDwFontFactoryWrapper
{
    IDwFontCollectionWrapper GetSystemFontCollection(bool checkForUpdates);
}