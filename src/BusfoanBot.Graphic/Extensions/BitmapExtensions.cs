using System.Drawing;
using System.IO;
using BusfoanBot.Graphic.Models;

namespace BusfoanBot.Graphic.Extensions
{
    public static class BitmapExtensions
    {
        public static Stream AsStream(this Bitmap image)
        {
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            memoryStream.Position = 0;

            return memoryStream;
        }

        public static PaddableImage WithNoPadding(this Bitmap image) 
            => new PaddableImage(image, Padding.All(0));

        public static PaddableImage WithPadding(this Bitmap image, Padding padding)
            => new PaddableImage(image, padding);
    }
}
