using System;
using System.ComponentModel.DataAnnotations;

namespace Etapa2.Models
{
    public class Card : Entity
    {
        public int BoardId { get; set; }
        public int BoardListId { get; set; }

        [Required]
        [Display(Name = "BoardList Position")]
        public int BoardListPosition { get; set; }

        public bool IsArchived { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Conclusion Date")]
        public DateTime DateConclusion { get; set; }
    }
}
