using System.Windows;
using System.Windows.Media.Imaging;

namespace LightlyShot.Capture;
internal sealed record MonitorShot(Int32Rect Bounds, BitmapSource Bitmap);
