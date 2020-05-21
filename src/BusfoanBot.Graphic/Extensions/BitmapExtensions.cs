using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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

        public static int Width(this IEnumerable<Bitmap> images)
            => images.Sum(i => i.Width);

        public static int Height(this IEnumerable<Bitmap> images)
            => images.Sum(i => i.Height);
    }
}
