using System;
using System.Drawing;
using System.Linq;
using BusfoanBot.Graphic.Models;

namespace BusfoanBot.Graphic.Services
{
    public enum Direction
    {
        Start, End, Center
    }

    public class MergeOptions
    {
        public Padding Padding { get; set; } = Padding.None;
        public int Gap { get; set; } = 0;
        public Direction Direction { get; set; } = Direction.Center;
    }

    public static class ImageUtil
    {
        public static Bitmap Vertical(params Bitmap[] images)
            => Vertical(null, images);
        public static Bitmap Vertical(MergeOptions options, params Bitmap[] images)
        {
            if (images == null || images.Count() == 0) return null;
            options = options ?? new MergeOptions();

            int width = images.Select(i => i.Width).Max()
                      + options.Padding.Width;

            int height = images.Sum(i => i.Height)
                      + options.Padding.Height
                      + (options.Gap * (images.Count() - 1)); // only between images

            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                int heightOffset = options.Padding.Top;

                foreach (var image in images)
                {
                    int widthOffset = options.Direction switch
                    {
                        Direction.Start => options.Padding.Left,
                        Direction.Center => (width - image.Width) / 2,
                        Direction.End => width - image.Width - options.Padding.Right,
                        _ => throw new NotImplementedException()
                    };

                    g.DrawImage(image,
                        widthOffset, heightOffset,
                        image.Width, image.Height);

                    heightOffset += image.Height + options.Gap;
                }
            }

            return bitmap;
        }

        public static Bitmap Horizontal(params Bitmap[] images)
            => Horizontal(null, images);

        public static Bitmap Horizontal(MergeOptions options, params Bitmap[] images)
        {
            if (images == null || images.Count() == 0) return null;

            options = options ?? new MergeOptions();

            int width = images.Sum(i => i.Width)                        
                      + options.Padding.Width
                      + (options.Gap * (images.Count() - 1)); // between images

            int height = images.Select(i => i.Height).Max() 
                      + options.Padding.Height;
            
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                int widthOffset = options.Padding.Left;

                foreach (var image in images)
                {
                    int heightOffset = options.Direction switch
                    {
                        Direction.Start => options.Padding.Top,
                        Direction.Center => (height - image.Height) / 2,
                        Direction.End => height - image.Height - options.Padding.Bottom,
                        _ => throw new NotImplementedException()
                    };

                    g.DrawImage(image, 
                        widthOffset, heightOffset, 
                        image.Width, image.Height);

                    widthOffset += image.Width + options.Gap;
                }
            }

            return bitmap;
        }
    }
}
