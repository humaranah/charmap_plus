using CharMapPlus.Infrastructure.DirectWrite;

namespace CharMapPlus.Infrastructure.IntegrationTests.TestHelpers;

public static class DWriteFontCollectionFactoryHelper
{
    public static DWriteFontCollectionFactory Instance { get; } = new();
}
