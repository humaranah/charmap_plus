using CharMapPlus.Core.Models;

namespace CharMapPlus.Core;

/// <summary>
/// Service for retrieving font information and characters.
/// </summary>
public interface IFontService
{
    /// <summary>
    /// Gets all installed font names.
    /// </summary>
    /// <returns>
    /// A collection of installed font names.
    /// </returns>
    ICollection<string> GetAllFonts();

    /// <summary>
    /// Gets all characters supported by the specified font.
    /// </summary>
    /// <param name="fontName">
    /// The name of the font to query.
    /// </param>
    /// <returns>
    /// A collection of <see cref="GlyphInfo"/> representing the characters supported by the font.
    /// </returns>
    ICollection<GlyphInfo> GetFontSupportedCharacters(string fontName);
}
