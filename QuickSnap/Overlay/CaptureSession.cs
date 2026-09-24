using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LightlyShot.Capture;
using LightlyShot.Editor;
using LightlyShot.Interop;
using LightlyShot.Output;
using LightlyShot.Settings;
using LightlyShot.Tray;

namespace LightlyShot.Overlay;

internal enum FinishAction
{
    CopyToClipboard,
    SaveToFile,
}
internal sealed class CaptureSession
{
    private static CaptureSession? currentSession;

    private readonly List<OverlayWindow> overlays = new();
    private readonly AppSettings settings;
    private readonly IUserNotifier notifier;

    public CaptureSession(AppSettings settings, IUserNotifier notifier)
    {
        this.settings = settings;
        this.notifier = notifier;
    }

    public AppSettings Settings => settings;

    public static bool IsOpen => currentSession is not null;
    public static void AbortCurrentSession() => currentSession?.CloseAllOverlays();
    public async Task StartAsync()
    {
        currentSession = this;

        try
        {
            IReadOnlyList<MonitorShot> shots = await Task.Run(() => ScreenCapturer.CaptureAllMonitors(settings.CaptureCursor));

            foreach (MonitorShot shot in shots)
            {
                overlays.Add(new OverlayWindow(shot, this));
            }

            foreach (OverlayWindow overlay in overlays)
            {
                overlay.Show();
            }

            FindOverlayUnderCursor()?.BringToFront();
        }
        catch
        {
            CloseAllOverlays();
            throw;
        }
    }
    public void StartWithPreviousCapture(LastCapture previous)
    {
        currentSession = this;

        try
        {
            var overlay = new OverlayWindow(previous.MonitorShot, this);
            overlays.Add(overlay);
            overlay.Show();
            overlay.Dispatcher.InvokeAsync(
                () => overlay.RestoreAnnotatedSelection(previous.Selection, previous.Annotations),
                DispatcherPriority.Loaded);

            overlay.BringToFront();
        }
        catch
        {
            CloseAllOverlays();
            throw;
        }
    }

    public void NotifySelectionStarting(OverlayWindow starter)
    {
        foreach (OverlayWindow overlay in overlays)
        {
            if (overlay != starter) overlay.ResetSelection();
        }
    }

    public void Cancel() => CloseAllOverlays();
    public void Finish(BitmapSource finalImage, FinishAction action)
    {
        CloseAllOverlays();

        if (action == FinishAction.CopyToClipboard)
        {
            _ = CopyToClipboardAsync(finalImage);
        }
        else
        {
            _ = SaveToFileAsync(finalImage);
        }
    }

    private async Task CopyToClipboardAsync(BitmapSource image)
    {
        try
        {
            await ClipboardService.CopyImageAsync(image);
        }
        catch (Exception problem)
        {
            notifier.ShowError("Couldn't copy the screenshot", problem.Message);
        }
    }

    private async Task SaveToFileAsync(BitmapSource image)
    {
        try
        {
            string savedPath = await ScreenshotSaver.SaveAsync(image, settings);

            if (settings.ShowSaveNotification)
            {
                notifier.ShowInfo("Screenshot saved", savedPath);
            }
        }
        catch (Exception problem)
        {
            notifier.ShowError("Couldn't save the screenshot", problem.Message);
        }
    }

    private OverlayWindow? FindOverlayUnderCursor()
    {
        NativeMethods.GetCursorPos(out NativeMethods.NativePoint cursor);

        foreach (OverlayWindow overlay in overlays)
        {
            if (overlay.ContainsScreenPoint(cursor.X, cursor.Y)) return overlay;
        }

        return overlays.Count > 0 ? overlays[0] : null;
    }

    private void CloseAllOverlays()
    {
        foreach (OverlayWindow overlay in overlays)
        {
            overlay.Close();
        }

        overlays.Clear();
        currentSession = null;
        Application.Current.Dispatcher.InvokeAsync(
            () => GC.Collect(2, GCCollectionMode.Optimized, blocking: false, compacting: true),
            DispatcherPriority.ApplicationIdle);
    }
}
