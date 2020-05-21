using System.Drawing;
using Busfoan.Graphic.Models;
using Xunit;
using static Busfoan.Graphic.Util.ImageUtil;

// TODO: change project folder
namespace Busfoan.Graphic.Test.Util
{
    public class ImageUtilTest
    {
        [Fact]
        public void TestHorizontalYAlign()
        {
            var i100 = new Bitmap("100x100.png");
            var i200 = new Bitmap("200x200.png");

            var hTop = Horizontal(new MergeOptions { YAlign = YAlign.Top, MinWidth = 300, MinHeight = 300 }, i100, i200);
            hTop.Save("h_y_1top.png");

            var hCenter = Horizontal(new MergeOptions { YAlign = YAlign.Center, MinWidth = 300, MinHeight = 300 }, i100, i200);
            hCenter.Save("h_y_2center.png");

            var hBottom = Horizontal(new MergeOptions { YAlign = YAlign.Bottom, MinWidth = 300, MinHeight = 300 }, i100, i200);
            hBottom.Save("h_y_3bottom.png");
        }

        [Fact]
        public void TestHorizontalXAlign()
        {
            var i11 = new Bitmap("100x100.png");
            var i12 = new Bitmap("100x100.png");

            var hLeft = Horizontal(new MergeOptions { XAlign = XAlign.Left, MinWidth = 300, MinHeight = 300 }, i11, i12);
            hLeft.Save("h_x_1left.png");

            var hCenter = Horizontal(new MergeOptions { XAlign = XAlign.Center, MinWidth = 300, MinHeight = 300 }, i11, i12);
            hCenter.Save("h_x_2center.png");

            var hRight = Horizontal(new MergeOptions { XAlign = XAlign.Right, MinWidth = 300, MinHeight = 300 }, i11, i12);
            hRight.Save("h_x_3right.png");
        }

        [Fact]
        public void TestVerticalYAlign()
        {
            var i100 = new Bitmap("100x100.png");
            var i200 = new Bitmap("200x200.png");

            var vTop = Vertical(new MergeOptions { YAlign = YAlign.Top, MinWidth = 400, MinHeight = 400 }, i100, i200);
            vTop.Save("v_y_1top.png");
            Assert.Equal(400, vTop.Width);
            Assert.Equal(400, vTop.Height);

            var vCenter = Vertical(new MergeOptions { YAlign = YAlign.Center, MinWidth = 400, MinHeight = 400 }, i100, i200);
            vCenter.Save("v_y_2center.png");
            Assert.Equal(400, vCenter.Width);
            Assert.Equal(400, vCenter.Height);

            var vBottom = Vertical(new MergeOptions { YAlign = YAlign.Bottom, MinWidth = 400, MinHeight = 400 }, i100, i200);
            vBottom.Save("v_y_3bottom.png");
            Assert.Equal(400, vBottom.Width);
            Assert.Equal(400, vBottom.Height);
        }

        [Fact]
        public void TestVerticalXAlign()
        {
            var i100 = new Bitmap("100x100.png");
            var i200 = new Bitmap("200x200.png");

            var vLeft = Vertical(new MergeOptions { XAlign = XAlign.Left, MinWidth = 400, MinHeight = 400 }, i100, i200);
            vLeft.Save("v_x_1left.png");
            Assert.Equal(400, vLeft.Width);
            Assert.Equal(400, vLeft.Height);

            var vCenter = Vertical(new MergeOptions { XAlign = XAlign.Center, MinWidth = 400, MinHeight = 400 }, i100, i200);
            vCenter.Save("v_x_2center.png");
            Assert.Equal(400, vCenter.Width);
            Assert.Equal(400, vCenter.Height);

            var vRight = Vertical(new MergeOptions { XAlign = XAlign.Right, MinWidth = 400, MinHeight = 400 }, i100, i200);
            vRight.Save("v_x_3right.png");
            Assert.Equal(400, vRight.Width);
            Assert.Equal(400, vRight.Height);
        }

        [Fact]
        public void TestHorizontalMinWidth()
        {
            var i100 = new Bitmap("100x100.png");

            var hminW50 = Horizontal(new MergeOptions { MinWidth = 50, XAlign = XAlign.Left }, i100);
            hminW50.Save("h_minW50.png");
            Assert.Equal(100, hminW50.Width);
            Assert.Equal(100, hminW50.Height);

            var hminW200 = Horizontal(new MergeOptions { MinWidth = 200, XAlign = XAlign.Left }, i100);
            hminW200.Save("h_minW200.png");
            Assert.Equal(200, hminW200.Width);
            Assert.Equal(100, hminW200.Height);

            var hminW300 = Horizontal(new MergeOptions { MinWidth = 300, XAlign = XAlign.Left }, i100);
            hminW300.Save("h_minW300.png");
            Assert.Equal(300, hminW300.Width);
            Assert.Equal(100, hminW300.Height);
        }

        [Fact]
        public void TestHorizontalMinHeight()
        {
            var i100 = new Bitmap("100x100.png");

            var hminH50 = Horizontal(new MergeOptions { MinHeight = 50, YAlign = YAlign.Top }, i100);
            hminH50.Save("h_minH50.png");
            Assert.Equal(100, hminH50.Width);
            Assert.Equal(100, hminH50.Height);

            var hminH200 = Horizontal(new MergeOptions { MinHeight = 200, YAlign = YAlign.Top }, i100);
            hminH200.Save("h_minH200.png");
            Assert.Equal(100, hminH200.Width);
            Assert.Equal(200, hminH200.Height);

            var hminH300 = Horizontal(new MergeOptions { MinHeight = 300, YAlign = YAlign.Top }, i100);
            hminH300.Save("h_minH300.png");
            Assert.Equal(100, hminH300.Width);
            Assert.Equal(300, hminH300.Height);
        }

        [Fact]
        public void TestText()
        {
            var i100 = new Bitmap("100x100.png");

            var textSmall = Vertical(
                Text(new TextOptions { Width = 100, FontSize = 26 }, "jsmall"),
                i100);
            textSmall.Save("text_small.png");

            var textLong = Vertical(
                Text(new TextOptions { Width = 100, FontSize = 14 }, "loooong"),
                i100);
            textLong.Save("text_long.png");
        }

        [Fact]
        public void PadTest()
        {
            var i200 = new Bitmap("200x200.png");
            
            var pad100 = Pad(i200, Padding.All(100));
            pad100.Save("v_p100.png");
            Assert.Equal(400, pad100.Width);
            Assert.Equal(400, pad100.Height);

            var padr200 = Pad(i200, Padding.RightPadding(200));
            padr200.Save("v_pr200.png");
            Assert.Equal(400, padr200.Width);
            Assert.Equal(200, padr200.Height);
        }

        [Fact]
        public void GeneratePyramid()
        {
            var card = new Bitmap("card.png");

            var style = new MergeOptions { Gap = 10, XAlign = XAlign.Center };
            var pyramid = Vertical(
                style,
                Horizontal(style, card),
                Horizontal(style, card, card),
                Horizontal(style, card, card, card),
                Horizontal(style, card, card, card, card));

            pyramid.Save("pyramid.png");
        }

        [Fact]
        public void GeneratePlayerCards()
        {
            var card = new Bitmap("card.png");

            var gap10 = new MergeOptions { Gap = 10 };
            var gap20 = new MergeOptions { Gap = 20 };
            var gap40 = new MergeOptions { Gap = 40 };

            var textStyle = new TextOptions 
            { 
                FontSize = 64, 
                Color = Brushes.White,
                Width = card.Width * 4 
            };

            var player1 = Vertical(
                Text(textStyle, "player1"),
                Horizontal(gap10, card, card, card, card));
            var player2 = Vertical(
                Text(textStyle, "player2"),
                Horizontal(gap10, card, card, card, card));

            var cards = Vertical(
                gap20,
                Horizontal(gap40, player1, player2),
                Horizontal(gap40, player1, player2),
                Horizontal(gap40, player1, player2),
                player1);

            cards.Save("pyramid_cards.png");
        }
    }
}
