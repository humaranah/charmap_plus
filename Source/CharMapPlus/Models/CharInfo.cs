using System.Drawing;
using System.Globalization;

namespace CharMapPlus.Models;

public record CharInfo(
    string Character,
    string Name,
    int CodePoint,
    UnicodeCategory Category,
    FontFamily FontFamily
);