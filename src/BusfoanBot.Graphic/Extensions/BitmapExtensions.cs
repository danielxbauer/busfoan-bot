using System.Drawing;
using System.IO;
using BusfoanBot.Graphic.Models;

namespace BusfoanBot.Graphic.Extensions
{
    internal static class BitmapExtensions
    {
        internal static Stream AsStream(this Bitmap image)
        {
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Position = 0;

            return memoryStream;
        }

        internal static PaddableImage WithNoPadding(this Bitmap image) 
            => new PaddableImage(image, Padding.All(0));

        internal static PaddableImage WithPadding(this Bitmap image, Padding padding)
            => new PaddableImage(image, padding);
    }
}
