using System.Windows.Media;

namespace LightlyShot.Editor.Annotations;
internal static class DrawingTools
{
    public static Brush CreateBrush(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }

    public static Pen CreatePen(Color color, double thickness, PenLineCap lineCap = PenLineCap.Round)
    {
        var pen = new Pen(CreateBrush(color), thickness)
        {
            StartLineCap = lineCap,
            EndLineCap = lineCap,
            LineJoin = PenLineJoin.Round,
        };
        pen.Freeze();
        return pen;
    }
}
