using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snappo.Editor;

internal static class ImagePixelator
{
    private const int BytesPerPixel = 4;

    public static BitmapSource Pixelate(BitmapSource source, int blockSize)
    {
        blockSize = Math.Max(2, blockSize);

        if (source.Format != PixelFormats.Bgr32)
        {
            source = new FormatConvertedBitmap(source, PixelFormats.Bgr32, null, 0);
        }

        int width = source.PixelWidth;
        int height = source.PixelHeight;
        int stride = width * BytesPerPixel;

        byte[] sourcePixels = new byte[stride * height];
        source.CopyPixels(sourcePixels, stride, 0);

        int blocksAcross = (width + blockSize - 1) / blockSize;
        int blocksDown = (height + blockSize - 1) / blockSize;
        byte[] blockPixels = new byte[blocksAcross * blocksDown * BytesPerPixel];

        Parallel.For(0, blocksDown, blockRow =>
        {
            int top = blockRow * blockSize;
            int bottom = Math.Min(top + blockSize, height);

            for (int blockColumn = 0; blockColumn < blocksAcross; blockColumn++)
            {
                int left = blockColumn * blockSize;
                int right = Math.Min(left + blockSize, width);

                long blueTotal = 0, greenTotal = 0, redTotal = 0;

                for (int y = top; y < bottom; y++)
                {
                    int index = y * stride + left * BytesPerPixel;
                    for (int x = left; x < right; x++)
                    {
                        blueTotal += sourcePixels[index];
                        greenTotal += sourcePixels[index + 1];
                        redTotal += sourcePixels[index + 2];
                        index += BytesPerPixel;
                    }
                }

                int pixelCount = (bottom - top) * (right - left);
                int outputIndex = (blockRow * blocksAcross + blockColumn) * BytesPerPixel;

                blockPixels[outputIndex] = (byte)(blueTotal / pixelCount);
                blockPixels[outputIndex + 1] = (byte)(greenTotal / pixelCount);
                blockPixels[outputIndex + 2] = (byte)(redTotal / pixelCount);
                blockPixels[outputIndex + 3] = 255;
            }
        });

        BitmapSource result = BitmapSource.Create(
            blocksAcross, blocksDown, 96, 96, PixelFormats.Bgr32, null, blockPixels, blocksAcross * BytesPerPixel);
        result.Freeze();
        return result;
    }
}
