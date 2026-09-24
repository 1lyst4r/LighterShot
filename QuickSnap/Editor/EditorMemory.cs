using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace LightlyShot.Editor;
internal sealed record RememberedSelection(Int32Rect MonitorBounds, Int32Rect PixelArea);
internal static class EditorMemory
{
    public static EditorTool? LastTool { get; set; }
    public static RememberedSelection? LastSelection { get; set; }

    public static Dictionary<EditorTool, Color> ColorForEachTool { get; } = EditorDefaults.CreateStartingColors();
}
