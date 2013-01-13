using System.Collections.Generic;
using Etapa2.Models;

namespace Etapa2.Views.Entitys
{
    public class ListDetails
    {
        public BoardList list { get; set; }
        public IEnumerable<IEnumerable<Card>> cardsDivided { get; set; }
        public int cardsPerLine { get; set; }
        public int numberOfCards { get; set; }
        public bool canUserWrite { get; set; }
    }
}