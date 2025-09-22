using Vortice.DirectWrite;

namespace CharMapPlus.Infrastructure.DirectWrite
{
    public class DwFontFamilyWrapper(uint index, IDWriteFontFamily fontFamily) : IDwFontFamilyWrapper
    {
        public uint Index { get; } = index;

        public uint FontCount => fontFamily.FontCount;

        public string GetFamilyName()
        {
            return fontFamily.FamilyNames.GetString(0);
        }

        public IDwLocalizedStringsWrapper FamilyNames => new DwLocalizedStringsWrapper(fontFamily.FamilyNames);

        public IEnumerable<IDwFontWrapper> GetFonts()
        {
            for (int i = 0; i < FontCount; i++)
            {
                yield return GetFont((uint)i);
            }
        }

        public IDwFontWrapper GetFont(uint index)
        {
            var font = fontFamily.GetFont(index);
            return new DwFontWrapper(index, font);
        }
    }
}
