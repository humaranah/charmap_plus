using Vortice.DirectWrite;

namespace CharMapPlus.Infrastructure.DirectWrite;

public class DwLocalizedStringsWrapper(IDWriteLocalizedStrings localizedStrings) : IDwLocalizedStringsWrapper
{
    public uint Count => localizedStrings.Count;

    public string GetLocaleName(uint index) => localizedStrings.GetLocaleName(index);

    public string GetString(uint index) => localizedStrings.GetString(index);
}
