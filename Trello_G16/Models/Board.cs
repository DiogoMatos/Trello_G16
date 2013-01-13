using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Etapa2.Models
{
    public class Board : Entity
    {
        [Required]
        [DataType(DataType.MultilineText)]
        public String Description { get; set; }

        [Required]
        public String User { get; set; }

        public IDictionary<string, BoardPermission> permissions = new Dictionary<string, BoardPermission>();

        public bool CanEdit(string user)
        {
            return (this.User == user || (permissions.Keys.Contains(user) && permissions[user].CanWrite));
        }
    }
}
