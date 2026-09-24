using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LightlyShot.Settings;

namespace LightlyShot.Output;

internal static class ScreenshotSaver
{
    public static Task<string> SaveAsync(BitmapSource frozenImage, AppSettings settings)
    {
        string folder = settings.GetSaveFolderOrDefault();
        SaveImageFormat format = settings.ImageFormat;
        int jpegQuality = settings.GetJpegQualityInRange();
        string extension = settings.GetFileExtension();

        return Task.Run(() =>
        {
            Directory.CreateDirectory(folder);
            string filePath = BuildUnusedFilePath(folder, extension);

            BitmapEncoder encoder = format == SaveImageFormat.Jpeg
                ? new JpegBitmapEncoder { QualityLevel = jpegQuality }
                : new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(frozenImage));

            using FileStream fileStream = File.Create(filePath);
            encoder.Save(fileStream);

            return filePath;
        });
    }
    private static string BuildUnusedFilePath(string folder, string extension)
    {
        string baseName = $"Screenshot {DateTime.Now:yyyy-MM-dd HH-mm-ss}";
        string filePath = Path.Combine(folder, baseName + extension);

        for (int copyNumber = 2; File.Exists(filePath); copyNumber++)
        {
            filePath = Path.Combine(folder, $"{baseName} ({copyNumber}){extension}");
        }

        return filePath;
    }
}
