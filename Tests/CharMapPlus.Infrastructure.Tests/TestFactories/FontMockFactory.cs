using CharMapPlus.Infrastructure.DirectWrite;
using Moq;

namespace CharMapPlus.Infrastructure.Tests.TestFactories;

public static class FontMockFactory
{
    public static Mock<IDwFontFamilyWrapper> CreateFontFamily(string name, (string? Name, ushort[] Glyphs)[] fonts)
    {
        var familyMock = new Mock<IDwFontFamilyWrapper>();
        familyMock.Setup(f => f.GetFamilyName()).Returns(name);
        familyMock.SetupGet(f => f.FontCount).Returns((uint)fonts.Length);

        var fontsByFamily = CreateFontsByFamily(familyMock, fonts);
        familyMock.Setup(f => f.GetFonts()).Returns(fontsByFamily);

        return familyMock;
    }

    private static IDwFontWrapper[] CreateFontsByFamily(
        Mock<IDwFontFamilyWrapper> familyMock, (string? Name, ushort[] Glyphs)[] fonts)
    {
        var count = fonts.Length;
        if (count == 0)
            return [];

        var results = new IDwFontWrapper[count];
        for (uint fontIndex = 0; fontIndex < count; fontIndex++)
        {
            var font = fonts[fontIndex];
            var fontName = font.Name;
            var fontMock = font switch
            {
                (null, _) => CreateFont(fontIndex, null, false),
                (_, var glyphs) => CreateFont(fontIndex, fontName!, glyphs)
            };

            familyMock.Setup(f => f.GetFont(fontIndex)).Returns(fontMock.Object);
            results[fontIndex] = fontMock.Object;
        }

        return results;
    }

    private static Mock<IDwFontWrapper> CreateFont(uint index, string fontName, ushort[] glyphs)
    {
        var mockFont = CreateFont(index, fontName, true);
        var mockFontFace = new Mock<IDwFontFaceWrapper>();

        mockFont.Setup(f => f.CreateFontFace()).Returns(mockFontFace.Object);
        mockFontFace.SetupGet(ff => ff.GlyphCount).Returns((ushort)glyphs.Length);

        if (glyphs.Length == 0)
            return mockFont;

        mockFontFace.Setup(ff => ff.GetGlyphIndices(It.IsAny<uint[]>()))
            .Returns((uint[] codePoints) =>
            {
                var result = new ushort[codePoints.Length];
                for (int i = 0; i < codePoints.Length; i++)
                {
                    var glyph = (ushort)codePoints[i];
                    result[i] = glyphs.Contains(glyph) ? glyph : (ushort)0;
                }
                return result;
            });

        return mockFont;
    }

    private static Mock<IDwFontWrapper> CreateFont(uint index, string? fontName, bool exists)
    {
        var mockFont = new Mock<IDwFontWrapper>();
        mockFont.SetupGet(f => f.Index).Returns(index);
        mockFont.Setup(f => f.TryGetFullName(out It.Ref<string?>.IsAny))
            .Returns((out string? name) =>
            {
                name = fontName;
                return exists;
            });
        return mockFont;
    }
}
