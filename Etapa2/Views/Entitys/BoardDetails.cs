using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Etapa2.Models;

namespace Etapa2.Views.Entitys
{
    public class BoardDetails
    {
        public Board board { get; set; }
        public IEnumerable<IEnumerable<BoardList>> listsDivided { get; set; }
        public int listsPerLine { get; set; }
        public int numberOfLists { get; set; }
        public bool canUserWrite { get; set; }
    }
}