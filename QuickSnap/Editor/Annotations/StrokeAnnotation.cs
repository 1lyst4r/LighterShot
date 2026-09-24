using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace LightlyShot.Editor.Annotations;
internal sealed class StrokeAnnotation : DragAnnotation
{
    private const double MinimumPointSpacing = 1.0;   // skip points closer than this, it keeps long strokes light

    private readonly List<Point> points = new();
    private readonly Pen pen;
    private Geometry? cachedShape;

    public StrokeAnnotation(Point startPoint, Pen pen) : base(startPoint)
    {
        this.pen = pen;
        points.Add(startPoint);
    }
    public override bool IsWorthKeeping => points.Count > 1 || pen.StartLineCap == PenLineCap.Round;

    public override void UpdateDrag(Point currentPoint)
    {
        if ((currentPoint - points[^1]).Length < MinimumPointSpacing)
        {
            return;
        }

        points.Add(currentPoint);
        cachedShape = null;
    }

    public override void Draw(DrawingContext drawing, RenderContext context)
    {
        cachedShape ??= BuildSmoothShape(points);
        drawing.DrawGeometry(null, pen, cachedShape);
    }
    private static Geometry BuildSmoothShape(List<Point> linePoints)
    {
        var shape = new StreamGeometry();

        using (StreamGeometryContext context = shape.Open())
        {
            context.BeginFigure(linePoints[0], false, false);

            if (linePoints.Count == 1)
            {
                context.LineTo(new Point(linePoints[0].X + 0.01, linePoints[0].Y), true, true);   // tiny line so the round cap draws a dot
            }
            else
            {
                for (int index = 1; index < linePoints.Count - 1; index++)
                {
                    Point midpoint = new(
                        (linePoints[index].X + linePoints[index + 1].X) / 2,
                        (linePoints[index].Y + linePoints[index + 1].Y) / 2);

                    context.QuadraticBezierTo(linePoints[index], midpoint, true, true);
                }

                context.LineTo(linePoints[^1], true, true);
            }
        }

        shape.Freeze();
        return shape;
    }
}
