using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LightlyShot.Editor;
internal sealed record RenderContext(double PixelsPerDip, Rect ScreenBounds, Lazy<BitmapSource> PixelatedScreenshot, Rect SelectionBounds);
