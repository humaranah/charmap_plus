using CharMapPlus.Infrastructure.DirectWrite;
using Moq;
using System.Globalization;
using System.Threading;

namespace CharMapPlus.Infrastructure.Tests;

public class DWriteFontCollectionProviderTests
{
    private readonly Mock<IDwFontFactoryWrapper> _factoryMock = new();
    private readonly DWriteFontCollectionProvider _provider;

    public DWriteFontCollectionProviderTests()
    {
        _provider = new DWriteFontCollectionProvider(_factoryMock.Object);
    }

    [Fact]
    public void FontCollection_Should_Invoke_GetSystemFontCollection_Once()
    {
        // Arrange
        var fontCollectionMock = new Mock<IDwFontCollectionWrapper>();
        _factoryMock.Setup(f => f.GetSystemFontCollection(false)).Returns(fontCollectionMock.Object);
        // Act
        var firstAccess = _provider.FontCollection;
        var secondAccess = _provider.FontCollection; // Accessing again to test caching
        // Assert
        _factoryMock.Verify(f => f.GetSystemFontCollection(false), Times.Once);
        Assert.Same(fontCollectionMock.Object, firstAccess);
        Assert.Same(fontCollectionMock.Object, secondAccess);
    }

    [Fact]
    public void GetFontFamilies_Should_Return_Correct_Number_Of_Families()
    {
        // Arrange
        var fontCollectionMock = new Mock<IDwFontCollectionWrapper>();
        fontCollectionMock.Setup(fc => fc.FontFamilyCount).Returns(3);
        fontCollectionMock.Setup(fc => fc.GetFontFamily(It.IsAny<uint>()))
            .Returns<uint>(i => new Mock<IDwFontFamilyWrapper>().Object);
        _factoryMock.Setup(f => f.GetSystemFontCollection(false)).Returns(fontCollectionMock.Object);
        // Act
        var families = _provider.GetFontFamilies();
        // Assert
        Assert.Equal(3, families.Count);
    }

    [Fact]
    public void GetFontFamily_ShouldReturnFontFamily()
    {
        // Arrange
        var expected = new Mock<IDwFontFamilyWrapper>();
        var fontCollectionMock = new Mock<IDwFontCollectionWrapper>();
        fontCollectionMock.Setup(fc => fc.FontFamilyCount).Returns(1);
        fontCollectionMock.Setup(fc => fc.GetFontFamily(0)).Returns(expected.Object);
        _factoryMock.Setup(f => f.GetSystemFontCollection(false)).Returns(fontCollectionMock.Object);
        // Act
        var family = _provider.GetFontFamily(0);
        // Assert
        Assert.NotNull(family);
        Assert.Equal(expected.Object, family);
    }

    [Theory]
    [InlineData("en-US", "English Face Name")]
    [InlineData("fr-FR", "Nom de la police française")]
    [InlineData("es-ES", "Nombre de fuente en español")]
    [InlineData("de-DE", "English Face Name")]
    [InlineData(null, "English Face Name")]
    public void GetLocalizedFaceName_Should_Return_Correct_FaceName_Based_On_Culture(string? cultureName, string expected)
    {
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var fontMock = new Mock<IDwFontWrapper>();
            var faceNamesMock = new Mock<IDwLocalizedStringsWrapper>();
            faceNamesMock.Setup(fn => fn.Count).Returns(3);
            faceNamesMock.Setup(fn => fn.GetLocaleName(0)).Returns("en-US");
            faceNamesMock.Setup(fn => fn.GetString(0)).Returns("English Face Name");
            faceNamesMock.Setup(fn => fn.GetLocaleName(1)).Returns("fr-FR");
            faceNamesMock.Setup(fn => fn.GetString(1)).Returns("Nom de la police française");
            faceNamesMock.Setup(fn => fn.GetLocaleName(2)).Returns("es-ES");
            faceNamesMock.Setup(fn => fn.GetString(2)).Returns("Nombre de fuente en español");
            fontMock.Setup(f => f.FaceNames).Returns(faceNamesMock.Object);
            // Act
            var actual = _provider.GetLocalizedFaceName(fontMock.Object, cultureName);
            // Assert
            Assert.Equal(expected, actual);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }
}
