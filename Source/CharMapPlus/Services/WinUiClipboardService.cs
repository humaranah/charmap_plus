using CharMapPlus.Core.Abstrations;
using Windows.ApplicationModel.DataTransfer;

namespace CharMapPlus.Services;

public class WinUiClipboardService : IClipboardService
{
    public void SetText(string text)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(text);
        Clipboard.SetContent(dataPackage);
        Clipboard.Flush();
    }
}
