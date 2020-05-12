using System.Drawing;
using System.Linq;

namespace BusfoanBot.Graphic
{
    public sealed class Padding
    {
        public int Top { get; set; } = 0;
        public int Right { get; set; } = 0;
        public int Bottom { get; set; } = 0;
        public int Left { get; set; } = 0;

        public static Padding All(int padding)
            => new Padding { Top = padding, Right = padding, Bottom = padding, Left = padding };
    }

    public class MergeOptions
    {
        public Padding Padding { get; set; } = Padding.All(0);
        public int Gap { get; set; } = 0;
    }

    public class ImageUtil
    {
        public Bitmap MergeHorizontal(MergeOptions options, params Bitmap[] images)
        {
            if (images == null || images.Length == 0) return null;

            int width = images.Sum(i => i.Width)
                      + options.Padding.Right 
                      + options.Padding.Left
                      + (options.Gap * (images.Length - 1)); // between images

            int height = images.Select(i => i.Height).Max() 
                      + options.Padding.Top 
                      + options.Padding.Bottom;
            
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                int widthOffset = options.Padding.Left;
                int heightOffset = options.Padding.Top;

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
