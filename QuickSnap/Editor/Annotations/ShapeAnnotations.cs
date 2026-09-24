using System.Windows;
using System.Windows.Media;

namespace LightlyShot.Editor.Annotations;

internal sealed class RectangleAnnotation : TwoPointAnnotation
{
    private readonly Pen pen;

    public RectangleAnnotation(Point startPoint, Color color, double thickness) : base(startPoint)
    {
        pen = DrawingTools.CreatePen(color, thickness);
    }

    public override void Draw(DrawingContext drawing, RenderContext context)
    {
        drawing.DrawRectangle(null, pen, EffectiveBounds(context));
    }
}

internal sealed class EllipseAnnotation : TwoPointAnnotation
{
    private readonly Pen pen;

    public EllipseAnnotation(Point startPoint, Color color, double thickness) : base(startPoint)
    {
        pen = DrawingTools.CreatePen(color, thickness);
    }

    public override void Draw(DrawingContext drawing, RenderContext context)
    {
        Rect area = EffectiveBounds(context);
        Point center = new(area.X + area.Width / 2, area.Y + area.Height / 2);
        drawing.DrawEllipse(null, pen, center, area.Width / 2, area.Height / 2);
    }
}
internal sealed class BlurAnnotation : TwoPointAnnotation
{
    public BlurAnnotation(Point startPoint) : base(startPoint)
    {
    }

    protected override bool SupportsShiftConstraint => false;

    public override void Draw(DrawingContext drawing, RenderContext context)
    {
        Rect area = EffectiveBounds(context);
        if (area.Width < 1 || area.Height < 1)
        {
            return;
        }
        drawing.PushClip(new RectangleGeometry(area));
        drawing.DrawImage(context.PixelatedScreenshot.Value, context.ScreenBounds);
        drawing.Pop();
    }
}
