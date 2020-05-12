using System;
using System.Drawing;
using System.Linq;

namespace BusfoanBot.Graphic
{
    public class MergeOptions
    {
        public int Padding { get; set; } = 0;
        public int Gap { get; set; } = 0;
    }

    public class ImageUtil
    {
        public Bitmap MergeHorizontal(MergeOptions options, params Bitmap[] images)
        {
            if (images == null || images.Length == 0) return null;

            int width = images.Sum(i => i.Width)
                      + (options.Padding * 2) // top + bottom
                      + (options.Gap * images.Length - 1); // between images

            int height = images.Select(i => i.Height).Max() 
                      + (options.Padding * 2); // top + bottom
            
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                int widthOffset = options.Padding;
                int heightOffset = options.Padding;

                foreach (var image in images)
                {
                    g.DrawImage(image, widthOffset, heightOffset, image.Width, image.Height);
                    widthOffset += image.Width + options.Gap;
                }
            }

            return bitmap;
        }
    }
}
