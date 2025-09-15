using Microsoft.UI.Xaml.Media;

namespace CharMapPlus.Converters;

public static partial class FontFamilyConverter
{
    public static FontFamily? ConvertFrom(string? fontName) =>
        fontName is null ? null : new FontFamily(fontName);
}
