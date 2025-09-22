using System.Diagnostics.CodeAnalysis;

namespace CharMapPlus.Infrastructure.DirectWrite;

public interface IDwFontWrapper
{
    uint Index { get; }
    IDwLocalizedStringsWrapper FaceNames { get; }
    bool TryGetFullName([NotNullWhen(true)] out string? fullName);
    IDwFontFaceWrapper CreateFontFace();
}
