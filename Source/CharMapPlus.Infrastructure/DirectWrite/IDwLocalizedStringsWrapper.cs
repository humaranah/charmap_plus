namespace CharMapPlus.Infrastructure.DirectWrite;

public interface IDwLocalizedStringsWrapper
{
    uint Count { get; }
    string GetLocaleName(uint index);
    string GetString(uint index);
}
