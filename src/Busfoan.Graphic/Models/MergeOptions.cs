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
}
