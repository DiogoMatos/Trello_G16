using System;
using System.ComponentModel.DataAnnotations;

namespace Etapa2.Models
{
    public abstract class Entity
    {
        public int Id { get; set; }
        
        [Required]
        public String Name { get; set; }
        public DateTime CreateDate { get; set; } 
    }
}
