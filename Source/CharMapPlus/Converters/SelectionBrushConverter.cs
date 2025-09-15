using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace CharMapPlus.Converters;

public static class SelectionBrushConverter
{
    public static SolidColorBrush Convert(bool isSelected)
    {
        var accentBrush = Application.Current.Resources["AccentFillColorDefaultBrush"] as SolidColorBrush;
        var defaultBrush = Application.Current.Resources["SystemControlBackgroundBaseLowBrush"] as SolidColorBrush;

        return (isSelected ? accentBrush : defaultBrush)!;
    }
}
