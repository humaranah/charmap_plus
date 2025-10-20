using CharMapPlus.Core.Models;
using System.Diagnostics.CodeAnalysis;

namespace CharMapPlus.Core.Abstrations;

public interface IFont
{
    uint Id { get; }
    bool TryGetFullName([NotNullWhen(true)] out string? fullName);
    ICollection<GlyphInfo> GetSupportedGlyphs();
}
