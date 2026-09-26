using System.Windows.Media;

namespace Snappo.Editor;

internal static class EditorDefaults
{
    // Selection overlay
    public const byte DimAmount = 140;                      // 0 = no dimming, 255 = solid black
    public const double SelectionCornerRadius = 7;
    public const double MinimumSelectionSize = 4;           // smaller drags count as an accidental click

    // Annotation tools
    public const double LineThickness = 3;
    public const double HighlightThickness = 18;
    public const byte HighlightOpacity = 100;               // 0-255
    public const double TextSize = 20;
    public const string TextFontName = "Segoe UI";
    public const double BlurBlockSize = 8;                  // bigger = stronger pixelation
    public const double MinimumShapeSize = 3;               // ignore tiny accidental drags

    // Colors
    public static readonly Color DefaultColor = Color.FromRgb(255, 59, 48);        // red

    public static readonly Color[] Palette =
    {
        Color.FromRgb(255, 59, 48),     // red
        Color.FromRgb(255, 214, 10),    // yellow
        Color.FromRgb(52, 199, 89),     // green
        Color.FromRgb(10, 132, 255),    // blue
        Color.FromRgb(255, 255, 255),   // white
        Color.FromRgb(0, 0, 0),         // black
    };

    public static readonly double[] TextSizes = { 14, 20, 28 };
}
