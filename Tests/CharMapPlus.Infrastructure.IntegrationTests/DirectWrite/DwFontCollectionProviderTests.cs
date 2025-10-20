using CharMapPlus.Infrastructure.DirectWrite;
using CharMapPlus.Infrastructure.IntegrationTests.TestHelpers;

namespace CharMapPlus.Infrastructure.IntegrationTests.DirectWrite;

[Trait("Category", "Integration")]
public class DwFontCollectionProviderTests
{
    private readonly DwFontCollectionProvider _provider = new(DWriteFontCollectionFactoryHelper.Instance);

    [Fact]
    public void GetFontFamilies_ShouldReturnAtLeastOneFamily()
    {
        // Act
        var families = _provider.GetFontFamilies();
        // Assert
        Assert.NotEmpty(families);
    }

    [Fact]
    public void GetFontFamily_ShouldReturnValidFamilyById()
    {
        // Arrange
        var families = _provider.GetFontFamilies();
        var firstFamily = families.First();
        // Act
        var retrievedFamily = _provider.GetFontFamily(firstFamily.Id);
        // Assert
        Assert.Equal(firstFamily.Id, retrievedFamily.Id);
    }
}
