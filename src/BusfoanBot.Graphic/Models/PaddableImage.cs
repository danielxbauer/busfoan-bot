using System;
using System.Drawing;

namespace BusfoanBot.Graphic.Models
{
    public sealed class PaddableImage
    {
        public PaddableImage(Bitmap image, Padding padding = null)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
            Padding = padding ?? Padding.None;
        }

        public Bitmap Image { get; }
        public Padding Padding { get; } = Padding.None;

        public int Width => Image.Width + Padding.Width;
        public int Height => Image.Height + Padding.Height;
    }
}
