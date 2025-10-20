using CharMapPlus.Core.Abstrations;
using CharMapPlus.Infrastructure.DirectWrite;
using CharMapPlus.Infrastructure.IntegrationTests.TestHelpers;

namespace CharMapPlus.Infrastructure.IntegrationTests.DirectWrite;

[Trait("Category", "Integration")]
public class DwFontTests
{
    private readonly IFontFamily _family;

    public DwFontTests()
    {
        var factory = DWriteFontCollectionFactoryHelper.Instance;
        var provider = new DwFontCollectionProvider(factory);
        _family = provider.GetFontFamilies().First();
    }

    [Fact]
    public void TryGetFullName_ShouldReturnTrueAndNonEmptyString()
    {
        // Arrange
        var fonts = _family.GetFonts();
        // Act
        var results = fonts.Select(f =>
        {
            var success = f.TryGetFullName(out var name);
            return (success, name);
        }).ToList();
        // Assert
        Assert.All(results, (result) =>
        {
            (var success, var name) = result;
            Assert.True(success);
            Assert.False(string.IsNullOrWhiteSpace(name));
        });
    }

    [Fact]
    public void GetSupportedGlyphs_ShouldReturnNonEmptyCollection()
    {
        // Arrange
        var fonts = _family.GetFonts();
        var font = fonts.First();
        // Act
        var glyphs = font.GetSupportedGlyphs();
        // Assert
        Assert.NotEmpty(glyphs);
    }
}
