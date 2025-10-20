using CharMapPlus.Core.Abstrations;
using CharMapPlus.Core.Models;
using CharMapPlus.Infrastructure.Tests.TestFactories;
using Moq;
using System.Globalization;

namespace CharMapPlus.Infrastructure.Tests;

[Trait("Category", "Unit")]
public class FontServiceTests
{
    private readonly Mock<IFontCollectionProvider> _mockProvider = new();
    private readonly FontService _fontService;

    private const string TestFontFamily = "Test Family";
    private const string TestFontName = "Test Font";

    public FontServiceTests()
    {
        _fontService = new(_mockProvider.Object);
    }

    #region LoadFonts Tests
    [Fact]
    public void LoadFonts_ShouldInitializeFontMap()
    {
        // Arrange
        _mockProvider.SetupFontFamilies([
            (TestFontFamily, [TestFontName])
        ]);
        // Act
        _fontService.LoadFonts();
        // Assert
        Assert.Single(_fontService.FontMap);
        Assert.True(_fontService.FontMap.ContainsKey(TestFontName));
    }

    [Fact]
    public void LoadFonts_ShouldNotAddDuplicateFontNames()
    {
        // Arrange
        _mockProvider.SetupFontFamilies([
            (TestFontFamily, [TestFontName, TestFontName]),
            (TestFontFamily, [TestFontName])
        ]);
        // Act
        _fontService.LoadFonts();
        // Assert
        Assert.Single(_fontService.FontMap);
        Assert.True(_fontService.FontMap.ContainsKey(TestFontName));
    }

    [Fact]
    public void LoadFonts_ShouldSkipNonExistingFontNames()
    {
        // Arrange
        _mockProvider.SetupFontFamilies([
            (TestFontFamily, [null, TestFontName]),
            ("Another Family", [])
        ]);
        // Act
        _fontService.LoadFonts();
        // Assert
        Assert.Single(_fontService.FontMap);
        Assert.True(_fontService.FontMap.ContainsKey(TestFontName));
    }
    #endregion

    #region ListFonts Tests
    [Fact]
    public void ListFonts_ShouldReturnEmptyCollection_WhenFontMapIsEmpty()
    {
        // Arrange
        _mockProvider.Setup(p => p.GetFontFamilies()).Returns([]);
        _fontService.LoadFonts();
        // Act
        var results = _fontService.ListFonts();
        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void ListFonts_ShouldReturnFontsCollection()
    {
        // Arrange
        _mockProvider.SetupFontFamilies([
            ("Arial", ["Arial", "Arial Black"]),
            ("Times New Roman", ["Times New Roman"])
        ]);
        _fontService.LoadFonts();
        // Act
        var results = _fontService.ListFonts();
        // Assert
        Assert.Equal(3, results.Count);
        Assert.Equal("Arial", results.ElementAt(0).Name);
        Assert.Equal("Arial Black", results.ElementAt(1).Name);
        Assert.Equal("Times New Roman", results.ElementAt(2).Name);
    }
    #endregion

    #region GetFontSupportedCharacters Tests
    [Fact]
    public void GetFontSupportedGlyphs_ShouldReturnEmptyArray_WhenFontNotFound()
    {
        // Arrange
        _mockProvider.Setup(p => p.GetFontFamilies()).Returns([]);
        _fontService.LoadFonts();
        // Act
        var result = _fontService.GetFontSupportedGlyphs("NonExistentFont");
        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetFontSupportedGlyphs_ShouldReturnEmptyArray_WhenFontHasNoGlyphs()
    {
        // Arrange
        _mockProvider.SetupFontFamilies([
            (TestFontFamily, [TestFontName])
        ]);
        _fontService.LoadFonts();
        // Act
        var result = _fontService.GetFontSupportedGlyphs(TestFontName);
        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetFontSupportedGlyphs_ShouldReturnSupportedCharacters()
    {
        // Arrange
        ushort[] glyphs = [0x0041, 0x0042, 0x0043]; // A, B, C
        var expected = glyphs.Select(g =>
        {
            var character = char.ConvertFromUtf32(g);
            return new GlyphInfo(character, g, g, UnicodeCategory.UppercaseLetter, TestFontName);
        }).ToArray();
        _mockProvider.SetupFontFamilies([
            (TestFontFamily, [
                (TestFontName, glyphs)
            ])
        ]);
        _fontService.LoadFonts();
        // Act
        var result = _fontService.GetFontSupportedGlyphs(TestFontName);
        // Assert
        Assert.Equal(expected, result);
    }
    #endregion
}
