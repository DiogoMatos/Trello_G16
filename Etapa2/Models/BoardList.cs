using System.ComponentModel.DataAnnotations;
using Etapa2.Repository;

namespace Etapa2.Models
{
    public class BoardList : Entity
    {
        [Required]
        [Display(Name = "Position")]
        public int BoardId { get; set; }
        public int BoardPosition { get; set; }
    }
}
