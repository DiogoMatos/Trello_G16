using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Etapa2.Models
{
    public class BoardPermission
    {
        public string User { get; set; }
        public bool CanWrite { get; set; }
    }
}