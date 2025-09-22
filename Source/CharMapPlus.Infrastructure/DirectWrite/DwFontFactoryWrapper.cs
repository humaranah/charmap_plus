using Vortice.DirectWrite;

namespace CharMapPlus.Infrastructure.DirectWrite;

public class DwFontFactoryWrapper : IDwFontFactoryWrapper
{
    private readonly IDWriteFactory _factory = DWrite.DWriteCreateFactory<IDWriteFactory>();

    public IDwFontCollectionWrapper GetSystemFontCollection(bool checkForUpdates)
    {
        var fontCollection = _factory.GetSystemFontCollection(checkForUpdates);
        return new DwFontCollectionWrapper(fontCollection);
    }
}
