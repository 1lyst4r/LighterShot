using System.Collections.Generic;
using System.Windows.Media;

namespace LightlyShot.Editor;
internal static class EditorDefaults
{
    public const byte DimAmount = 140;                      // 0 = no dimming, 255 = solid black
    public const double SelectionCornerRadius = 7;
    public const double MinimumSelectionSize = 4;           // smaller drags count as an accidental click
    public const double LineThickness = 3;
    public const double HighlightThickness = 18;
    public const byte HighlightOpacity = 100;               // 0-255
    public const double TextSize = 20;
    public const string TextFontName = "Segoe UI";
    public const double BlurBlockSize = 8;                  // bigger = stronger pixelation
    public const double MinimumShapeSize = 3;               // ignore tiny accidental drags
    public static readonly Color DefaultColor = Color.FromRgb(255, 59, 48);        // red
    public static readonly Color DefaultHighlightColor = Color.FromRgb(255, 214, 10); // yellow

    public static readonly Color[] Palette =
    {
        Color.FromRgb(255, 59, 48),     // red
        Color.FromRgb(255, 214, 10),    // yellow
        Color.FromRgb(52, 199, 89),     // green
        Color.FromRgb(10, 132, 255),    // blue
        Color.FromRgb(255, 255, 255),   // white
        Color.FromRgb(0, 0, 0),         // black
    };
    public static Dictionary<EditorTool, Color> CreateStartingColors()
    {
        var colors = new Dictionary<EditorTool, Color>();

        foreach (EditorTool tool in System.Enum.GetValues<EditorTool>())
        {
            colors[tool] = tool == EditorTool.Highlight ? DefaultHighlightColor : DefaultColor;
        }

        return colors;
    }
}
