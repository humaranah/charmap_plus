using Vortice.DirectWrite;

namespace CharMapPlus.Infrastructure.DirectWrite;

public class DWriteFontCollectionFactory : IDWriteFontCollectionFactory
{
    public IDWriteFontCollection Create()
    {
        var factory = DWrite.DWriteCreateFactory<IDWriteFactory>();
        return factory.GetSystemFontCollection(false);
    }
}
