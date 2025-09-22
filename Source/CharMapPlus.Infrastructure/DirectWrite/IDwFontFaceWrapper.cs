namespace CharMapPlus.Infrastructure.DirectWrite;

public interface IDwFontFaceWrapper
{
    ushort GlyphCount { get; }
    ushort[] GetGlyphIndices(IEnumerable<uint> codePoints);
}
