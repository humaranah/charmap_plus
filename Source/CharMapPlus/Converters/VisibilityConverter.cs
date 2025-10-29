using Microsoft.UI.Xaml;

namespace CharMapPlus.Converters;

public static class VisibilityConverter
{
    public static Visibility ToVisibility(this bool value)
    {
        return value ? Visibility.Visible : Visibility.Collapsed;
    }

    public static Visibility ToInvertedVisibility(this bool value)
    {
        return value ? Visibility.Collapsed : Visibility.Visible;
    }
}
