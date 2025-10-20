using CharMapPlus.Infrastructure.DirectWrite;
using CharMapPlus.Infrastructure.IntegrationTests.TestHelpers;

namespace CharMapPlus.Infrastructure.IntegrationTests.DirectWrite;

[Trait("Category", "Integration")]
public class DwFontFamilyTests
{
    private readonly DwFontCollectionProvider _provider = new(DWriteFontCollectionFactoryHelper.Instance);

    [Fact]
    public void GetFonts_ShouldReturnAtLeastOneFont()
    {
        // Arrange
        var families = _provider.GetFontFamilies();
        var firstFamily = families.First();
        // Act
        var fonts = firstFamily.GetFonts();
        // Assert
        Assert.NotEmpty(fonts);
    }

    [Fact]
    public void TryGetFont_ShouldReturnValidFontByIndex()
    {
        // Arrange
        var families = _provider.GetFontFamilies();
        var firstFamily = families.First();
        var fonts = firstFamily.GetFonts();
        var firstFont = fonts.First();
        // Act
        var success = firstFamily.TryGetFont(0, out var retrievedFont);
        // Assert
        Assert.True(success);
        Assert.NotNull(retrievedFont);
        Assert.Equal(firstFont.Id, retrievedFont.Id);
    }

    [Fact]
    public void TryGetFont_ShouldReturnFalseForInvalidIndex()
    {
        // Arrange
        var families = _provider.GetFontFamilies();
        var firstFamily = families.First();
        var invalidIndex = uint.MaxValue;
        // Act
        var success = firstFamily.TryGetFont(invalidIndex, out var retrievedFont);
        // Assert
        Assert.False(success);
        Assert.Null(retrievedFont);
    }

    [Fact]
    public void GetFamilyName_ShouldReturnNonEmptyName()
    {
        // Arrange
        var families = _provider.GetFontFamilies();
        var firstFamily = families.First();
        // Act
        var familyName = firstFamily.GetFamilyName();
        // Assert
        Assert.False(string.IsNullOrWhiteSpace(familyName));
    }
}
