using System.Drawing;

namespace Busfoan.Graphic.Models
{
    internal sealed class MergeOptions
    {
        public int MinWidth { get; set; } = 0;
        public int MinHeight { get; set; } = 0;
        public int Gap { get; set; } = 0;
        public XAlign XAlign { get; set; } = XAlign.Left;
        public YAlign YAlign { get; set; } = YAlign.Top;
    }

    internal sealed class TextOptions
    {
        public int FontSize { get; set; } = 16;
        public Brush Color { get; set; } = Brushes.Black;
        public int Width { get; set; } = 100;
    }
}
