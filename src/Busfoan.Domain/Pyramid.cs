using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Busfoan.Domain
{
    public class PyramidRow
    {
        public PyramidRow(int row, IEnumerable<Card> cards)
        {
            this.Row = row;
            this.Cards = ImmutableList.CreateRange(cards);
        }

        public int Row { get; }
        public ImmutableList<Card> Cards { get; }
    }

    public class Pyramid
    {
        public Pyramid(int rows, Func<Card> draw)
        {
            this.Rows = ImmutableList.CreateRange(InitRows(rows, draw));
        }

        private IEnumerable<PyramidRow> InitRows(int rows, Func<Card> draw)
        {
            for (int i = 1; i <= rows; i++)
            {
                yield return new PyramidRow(i,
                    Enumerable.Range(0, i).Select(_ => draw()).ToList());
            }
        }

        public ImmutableList<PyramidRow> Rows { get; }
        private IEnumerable<Card> Cards => Rows.OrderByDescending(r => r.Row).SelectMany(r => r.Cards);

        public void RevealCard()
        {
            Card card = Cards.Where(c => !c.IsRevealed).First();
            card.IsRevealed = true;
        }
        public bool CardsLeftToReveal
            => Cards.Any(c => !c.IsRevealed);
    }
}
