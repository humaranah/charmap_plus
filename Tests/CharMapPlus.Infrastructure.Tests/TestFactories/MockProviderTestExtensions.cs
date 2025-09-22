using CharMapPlus.Infrastructure.Abstractions;
using CharMapPlus.Infrastructure.DirectWrite;
using Moq;

namespace CharMapPlus.Infrastructure.Tests.TestFactories;

public static class MockProviderTestExtensions
{
    public static void SetupFontFamilies(
        this Mock<IFontCollectionProvider> mockProvider,
        (string Name, string?[] FontNames)[] families)
    {
        if (families.Length == 0)
            return;

        mockProvider.SetupFontFamilies(families.Select(
            family =>
            {
                var fonts = family.FontNames
                    .Select(n => (n, Array.Empty<ushort>()))
                    .ToArray();

                return (family.Name, fonts);
            }).ToArray());
    }

    public static void SetupFontFamilies(
        this Mock<IFontCollectionProvider> mockProvider,
        (string Name, (string? Name, ushort[] Glyphs)[] Fonts)[] families)
    {
        var count = families.Length;
        if (count == 0)
            return;

        var fontFamilies = new IDwFontFamilyWrapper[count];
        for (uint i = 0; i < count; i++)
        {
            (var name, var fonts) = families[i];
            var mockFamily = FontMockFactory.CreateFontFamily(name, fonts);
            mockFamily.SetupGet(f => f.Index).Returns(i);
            mockProvider.Setup(p => p.GetFontFamily(i)).Returns(mockFamily.Object);
            fontFamilies[i] = mockFamily.Object;
        }
        mockProvider.Setup(p => p.GetFontFamilies()).Returns(fontFamilies);
    }
}
