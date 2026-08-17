using CharMapPlus.Core.Abstrations;
using CharMapPlus.Core.Models;
using Moq;
using System.Globalization;

namespace CharMapPlus.Infrastructure.Tests.TestFactories;

public static class FontMockFactory
{
    public static Mock<IFontFamily> CreateFontFamily(string name, (string? Name, ushort[] Glyphs)[] fonts)
    {
        var familyMock = new Mock<IFontFamily>();
        familyMock.Setup(f => f.GetFamilyName()).Returns(name);

        var fontsByFamily = CreateFontsByFamily(familyMock, fonts);
        familyMock.Setup(f => f.GetFonts()).Returns(fontsByFamily);

        return familyMock;
    }

    private static IFont[] CreateFontsByFamily(
        Mock<IFontFamily> familyMock, (string? Name, ushort[] Glyphs)[] fonts)
    {
        var count = fonts.Length;
        if (count == 0)
            return [];

        var results = new IFont[count];
        for (uint fontIndex = 0; fontIndex < count; fontIndex++)
        {
            var font = fonts[fontIndex];
            var fontName = font.Name;
            var fontMock = font switch
            {
                (null, _) => CreateFont(fontIndex, null, false),
                (_, var glyphs) => CreateFont(fontIndex, fontName!, glyphs)
            };

            familyMock.Setup(f => f.TryGetFont(fontIndex, out It.Ref<IFont?>.IsAny))
                .Returns((uint _, out IFont? f) => { f = fontMock.Object; return true; });
            results[fontIndex] = fontMock.Object;
        }

        return results;
    }

    private static Mock<IFont> CreateFont(uint index, string fontName, ushort[] glyphs)
    {
        var mockFont = CreateFont(index, fontName, true);

        mockFont.Setup(f => f.GetSupportedGlyphs())
            .Returns(() =>
            {
                var result = new List<GlyphInfo>();
                foreach (var glyph in glyphs)
                {
                    var @char = char.ConvertFromUtf32(glyph);
                    result.Add(new GlyphInfo(@char, glyph, glyph, UnicodeCategory.UppercaseLetter));
                }
                return result;
            });
        return mockFont;
    }

    private static Mock<IFont> CreateFont(uint index, string? fontName, bool exists)
    {
        var mockFont = new Mock<IFont>();
        mockFont.SetupGet(f => f.Id).Returns(index);
        mockFont.Setup(f => f.TryGetFullName(out It.Ref<string?>.IsAny))
            .Returns((out string? name) =>
            {
                name = fontName;
                return exists;
            });
        return mockFont;
    }
}
