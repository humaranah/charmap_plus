using System.Diagnostics.CodeAnalysis;
using Vortice.DirectWrite;

namespace CharMapPlus.Infrastructure.DirectWrite;

public class DwFontWrapper(uint index, IDWriteFont font) : IDwFontWrapper
{
    public uint Index => index;

    public IDwLocalizedStringsWrapper FaceNames => new DwLocalizedStringsWrapper(font.FaceNames);

    public bool TryGetInformationalStrings(InformationalStringId informationalStringId,
        [NotNullWhen(true)] out IDwLocalizedStringsWrapper? informationalStrings)
    {
        font.GetInformationalStrings(informationalStringId, out var strings, out var exists);
        informationalStrings = exists && strings is not null ? new DwLocalizedStringsWrapper(strings) : null;
        return exists;
    }

    public IDwFontFaceWrapper CreateFontFace()
    {
        var fontFace = font.CreateFontFace();
        return new DwFontFaceWrapper(fontFace);
    }

    public bool TryGetFullName([NotNullWhen(true)] out string? fullName)
    {
        if (TryGetString(InformationalStringId.Win32FamilyNames, out fullName))
        {
            return true;
        }
        return false;
    }

    private bool TryGetString(InformationalStringId id, [NotNullWhen(true)] out string? result)
    {
        font.GetInformationalStrings(id, out var strings, out var exists);
        if (!exists || strings is null)
        {
            result = null;
            return false;
        }
        result = strings.GetString(0);
        return true;
    }
}