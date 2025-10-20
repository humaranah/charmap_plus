using System.Diagnostics.CodeAnalysis;

namespace CharMapPlus.Core.Abstrations;

public interface IFontFamily
{
    uint Id { get; }

    string GetFamilyName();
    IEnumerable<IFont> GetFonts();
    bool TryGetFont(uint fontId, [NotNullWhen(true)] out IFont? font);
}
