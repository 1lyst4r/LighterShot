using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Snappo.Editor;
using Snappo.Interop;

namespace Snappo.Tray;

internal sealed class TrayIcon : IUserNotifier, IDisposable
{
    private readonly NotifyIcon notifyIcon;
    private readonly Icon icon;
    private readonly IntPtr iconHandle;
    private readonly ToolStripMenuItem continueEditingItem;

    public TrayIcon(Action takeScreenshot, Action continueEditing, Action openScreenshotsFolder, Action openSettings, Action exitApp)
    {
        using (Bitmap iconBitmap = DrawIconBitmap())
        {
            iconHandle = iconBitmap.GetHicon();
        }

        icon = Icon.FromHandle(iconHandle);

        continueEditingItem = new ToolStripMenuItem("Continue editing last screenshot", null, (_, _) => continueEditing())
        {
            Enabled = false,
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add("Take screenshot", null, (_, _) => takeScreenshot());
        menu.Items.Add(continueEditingItem);
        menu.Items.Add("Open screenshots folder", null, (_, _) => openScreenshotsFolder());
        menu.Items.Add("Settings...", null, (_, _) => openSettings());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => exitApp());

        menu.Opening += (_, _) => continueEditingItem.Enabled = LastCaptureMemory.Value is not null;

        notifyIcon = new NotifyIcon
        {
            Icon = icon,
            Text = "Snappo",
            ContextMenuStrip = menu,
            Visible = true,
        };

        notifyIcon.MouseClick += (_, clickArgs) =>
        {
            if (clickArgs.Button == MouseButtons.Left) takeScreenshot();
        };
    }

    public void ShowInfo(string title, string message) =>
        notifyIcon.ShowBalloonTip(3000, title, message, ToolTipIcon.Info);

    public void ShowError(string title, string message) =>
        notifyIcon.ShowBalloonTip(5000, title, message, ToolTipIcon.Error);

    public void Dispose()
    {
        notifyIcon.Visible = false;
        notifyIcon.Dispose();
        icon.Dispose();
        NativeMethods.DestroyIcon(iconHandle);
    }

    private static Bitmap DrawIconBitmap()
    {
        const int size = 32;
        const int near = 8;
        const int far = 24;
        const int bracketLength = 6;

        var bitmap = new Bitmap(size, size);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using var background = new GraphicsPath();
        const int diameter = 10;
        background.AddArc(0, 0, diameter, diameter, 180, 90);
        background.AddArc(size - diameter - 1, 0, diameter, diameter, 270, 90);
        background.AddArc(size - diameter - 1, size - diameter - 1, diameter, diameter, 0, 90);
        background.AddArc(0, size - diameter - 1, diameter, diameter, 90, 90);
        background.CloseFigure();

        using var backgroundBrush = new SolidBrush(Color.FromArgb(10, 132, 255));
        graphics.FillPath(backgroundBrush, background);

        using var bracketPen = new Pen(Color.White, 3f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
            LineJoin = LineJoin.Round,
        };

        graphics.DrawLines(bracketPen, new[] { new Point(near, near + bracketLength), new Point(near, near), new Point(near + bracketLength, near) });
        graphics.DrawLines(bracketPen, new[] { new Point(far - bracketLength, near), new Point(far, near), new Point(far, near + bracketLength) });
        graphics.DrawLines(bracketPen, new[] { new Point(near, far - bracketLength), new Point(near, far), new Point(near + bracketLength, far) });
        graphics.DrawLines(bracketPen, new[] { new Point(far - bracketLength, far), new Point(far, far), new Point(far, far - bracketLength) });

        return bitmap;
    }
}
