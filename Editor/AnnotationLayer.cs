using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Snappo.Editor.Annotations;

namespace Snappo.Editor;

internal sealed class AnnotationLayer : FrameworkElement
{
    private readonly Func<RenderContext> getRenderContext;
    private IReadOnlyList<Annotation> annotationsToDraw = Array.Empty<Annotation>();

    public AnnotationLayer(Func<RenderContext> getRenderContext)
    {
        this.getRenderContext = getRenderContext;
        IsHitTestVisible = false;

        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
    }

    public void Show(IReadOnlyList<Annotation> annotations)
    {
        annotationsToDraw = annotations;
        InvalidateVisual();
    }

    public void Refresh() => InvalidateVisual();

    protected override void OnRender(DrawingContext drawing)
    {
        if (annotationsToDraw.Count == 0)
        {
            return;
        }

        RenderContext context = getRenderContext();
        foreach (Annotation annotation in annotationsToDraw)
        {
            annotation.Draw(drawing, context);
        }
    }
}
