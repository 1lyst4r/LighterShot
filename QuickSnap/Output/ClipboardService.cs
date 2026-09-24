using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LightlyShot.Output;

internal static class ClipboardService
{
    public static Task CopyImageAsync(BitmapSource frozenImage)
    {
        var finished = new TaskCompletionSource();

        var clipboardThread = new Thread(() =>
        {
            try
            {
                Clipboard.SetImage(frozenImage);   // copies with flush, so the image stays available after we exit
                finished.SetResult();
            }
            catch (Exception problem)
            {
                finished.SetException(problem);
            }
        });

        clipboardThread.SetApartmentState(ApartmentState.STA);
        clipboardThread.Start();

        return finished.Task;
    }
}
