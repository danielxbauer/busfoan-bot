namespace Busfoan.Graphic.Models
{
    internal sealed class Padding
    {
        public int Top { get; set; } = 0;
        public int Right { get; set; } = 0;
        public int Bottom { get; set; } = 0;
        public int Left { get; set; } = 0;

        public int Width => Right + Left;
        public int Height => Top + Bottom;

        public static Padding None => All(0);
        public static Padding TopPadding(int top)
            => new Padding { Top = top, Right = 0, Bottom = 0, Left = 0 };
        public static Padding All(int padding)
            => new Padding { Top = padding, Right = padding, Bottom = padding, Left = padding };
    }
}
