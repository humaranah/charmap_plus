using CharMapPlus.Core.Models;

namespace CharMapPlus.Core;

/// <summary>
/// Service for retrieving font information and characters.
/// </summary>
public interface IFontService
{
    /// <summary>
    /// 
    /// </summary>
    void LoadFonts();

    /// <summary>
    /// Gets all installed font names.
    /// </summary>
    /// <returns>
    /// A collection of installed font names.
    /// </returns>
    ICollection<FontInfo> ListFonts();

    /// <summary>
    /// Gets all characters supported by the specified font.
    /// </summary>
    /// <param name="fontName">
    /// The name of the font to query.
    /// </param>
    /// <returns>
    /// A collection of <see cref="GlyphInfo"/> representing the characters supported by the font.
    /// </returns>
    ICollection<GlyphInfo> GetFontSupportedGlyphs(string fontName);
}
