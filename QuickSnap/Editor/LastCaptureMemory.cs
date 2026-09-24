using System.Collections.Generic;
using System.Windows;
using LightlyShot.Capture;
using LightlyShot.Editor.Annotations;

namespace LightlyShot.Editor;

internal sealed record LastCapture(MonitorShot MonitorShot, Rect Selection, IReadOnlyList<Annotation> Annotations);
internal static class LastCaptureMemory
{
    public static LastCapture? Value { get; set; }
}
