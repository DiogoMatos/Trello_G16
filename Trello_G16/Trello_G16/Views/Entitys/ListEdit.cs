using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Etapa2.Models;

namespace Etapa2.Views.Entitys
{
    public class ListEdit
    {
        public BoardList list { get; set; }
        public string user { get; set; }
        public int numberofcardslistcontains { get; set; }
        public int numberoflistsinboard { get; set; }
    }
}