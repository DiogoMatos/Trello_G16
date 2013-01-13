using System.ComponentModel.DataAnnotations;

namespace Etapa2.Models
{
    public class User : Entity
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Nickname")]
        public string Nickname { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Type { get; set; }

        public byte[] Image { get; set; }
        public string ImageMimeType { get; set; }

        //Validar password
        public bool VerifyUserCredentials(string nick, string pass)
        {
            return Password.Equals(pass) && Nickname.Equals(nick);
        }

        public void SetAdmin()
        {
            Type = "admin";
        }
    }
}