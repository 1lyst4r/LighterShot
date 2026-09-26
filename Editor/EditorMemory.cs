using System.Windows;
using System.Windows.Media;

namespace Snappo.Editor;

internal sealed record RememberedSelection(Int32Rect MonitorBounds, Int32Rect PixelArea);

internal static class EditorMemory
{
    public static EditorTool? LastTool { get; set; }

    public static Color LastColor { get; set; } = EditorDefaults.DefaultColor;

    public static double LastTextSize { get; set; } = EditorDefaults.TextSize;

    public static bool ShapesAreFilled { get; set; }

    public static RememberedSelection? LastSelection { get; set; }
}
